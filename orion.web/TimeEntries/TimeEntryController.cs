using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeApproval;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{

    [Authorize]
    public class TimeEntryController : Controller
    {
        public const string EDIT_ROUTE = nameof(TimeEntryController) + nameof(Edit);
        //private readonly IWeekService weekService;
        private readonly ICopyPreviousWeekTimeCommand copyPreviousWeekTimeCommand;
        private readonly ISaveTimeEntriesCommand saveTimeEntriesCommand;
        private readonly IAddNewJobTaskComboCommand addNewJobTaskComboCommand;
        private readonly IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery;
        private readonly IWeekIdentifierListQuery weekIdentifierListQuery;
        private readonly IApproveTimeCommand approveTimeCommand;
        private readonly IRemoveRowCommand removeRowCommand;
        private readonly IModifyJobTaskComboCommand modifyJobTaskComboCommand;
        private readonly ISessionAdapter sessionAdapter;

        public TimeEntryController(//IWeekService weekService,
            ICopyPreviousWeekTimeCommand copyPreviousWeekTimeCommand,
            ISaveTimeEntriesCommand saveTimeEntriesCommand,
            IAddNewJobTaskComboCommand addNewJobTaskComboCommand,
            IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery,
            IWeekIdentifierListQuery weekIdentifierListQuery,
            IApproveTimeCommand approveTimeCommand,
            IRemoveRowCommand removeRowCommand,
            IModifyJobTaskComboCommand modifyJobTaskComboCommand,
            ISessionAdapter sessionAdapter)
        {
            //this.weekService = weekService;
            this.copyPreviousWeekTimeCommand = copyPreviousWeekTimeCommand;
            this.saveTimeEntriesCommand = saveTimeEntriesCommand;
            this.addNewJobTaskComboCommand = addNewJobTaskComboCommand;
            this.weekOfTimeEntriesQuery = weekOfTimeEntriesQuery;
            this.weekIdentifierListQuery = weekIdentifierListQuery;
            this.approveTimeCommand = approveTimeCommand;
            this.removeRowCommand = removeRowCommand;
            this.modifyJobTaskComboCommand = modifyJobTaskComboCommand;
            this.sessionAdapter = sessionAdapter;
        }

        public async Task<ActionResult> Index(int? weeksToShow, string startWithDate)
        {
            if(!DateTime.TryParse(startWithDate, out var startDate))
            {
                startDate = DateTime.Now;
            }
            return View("WeekList", await weekIdentifierListQuery.GetWeeksAsync(weeksToShow ?? 5, await sessionAdapter.EmployeeIdAsync(), startDate));
        }


        public async Task<ActionResult> Current()
        {
            var current = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            return RedirectToAction("Edit", new { weekId = current.WeekId.Value, employeeId = await sessionAdapter.EmployeeIdAsync() });
        }


        [HttpGet]
        [Route("Edit/Employee/{employeeId}/Week/{weekId:int=1}", Name = EDIT_ROUTE)]
        public async Task<ActionResult> Edit(int weekId, int employeeId)
        {
            var req = new WeekOfTimeEntriesRequest()
            {
                EmployeeId = employeeId,
                RequestingUserIsAdmin = User.IsInRole(UserRoleName.Admin),
                RequestingUserName = User.Identity.Name,
                WeekId = weekId
            };
            var vm = await weekOfTimeEntriesQuery.GetFullTimeEntryViewModelAsync(req);
            return View("Week", vm);
        }



        [HttpPost]
        [Route("Edit/Employee/{employeeId}/Week/{weekId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(int weekId, int employeeId, FullTimeEntryViewModel vm, string postType)
        {
            if(postType == "Save" || postType == "Add Task" || postType == "Submit")
            {
                var res = await saveTimeEntriesCommand.SaveTimeEntriesAsync(employeeId, weekId, vm);
                if(res.Successful)
                {
                    NotificationsController.AddNotification(User.SafeUserName(), "Timesheet has been saved");
                }
                else
                {
                    var req = new WeekOfTimeEntriesRequest()
                    {
                        EmployeeId = employeeId,
                        RequestingUserIsAdmin = User.IsInRole(UserRoleName.Admin),
                        RequestingUserName = User.Identity.Name,
                        WeekId = weekId
                    };
                    var vmDefault = await weekOfTimeEntriesQuery.GetFullTimeEntryViewModelAsync(req);
                    foreach(var day in vmDefault.TimeEntryRow)
                    {
                        var match = vm.TimeEntryRow.Single(x => x.RowId == day.RowId);

                        day.Monday.Hours = match.Monday.Hours;
                        day.Tuesday.Hours = match.Tuesday.Hours;
                        day.Wednesday.Hours = match.Wednesday.Hours;
                        day.Thursday.Hours = match.Thursday.Hours;
                        day.Friday.Hours = match.Friday.Hours;
                        day.Saturday.Hours = match.Saturday.Hours;
                        day.Sunday.Hours = match.Sunday.Hours;

                        day.Monday.OvertimeHours = match.Monday.OvertimeHours;
                        day.Tuesday.OvertimeHours = match.Tuesday.OvertimeHours;
                        day.Wednesday.OvertimeHours = match.Wednesday.OvertimeHours;
                        day.Thursday.OvertimeHours = match.Thursday.OvertimeHours;
                        day.Friday.OvertimeHours = match.Friday.OvertimeHours;
                        day.Saturday.OvertimeHours = match.Saturday.OvertimeHours;
                        day.Sunday.OvertimeHours = match.Sunday.OvertimeHours;
                    }
                    NotificationsController.AddNotification(User.SafeUserName(), $"Timesheet was not saved {string.Join("<br />", res.Errors)}");
                    ModelState.Clear();
                    foreach(var err in res.Errors)
                    {
                        ModelState.AddModelError("", err);
                    }
                    return View("Week", vmDefault);
                }
            }

            if(postType == "Add Task")
            {
                var res = await addNewJobTaskComboCommand.AddNewJobTaskCombo(employeeId, weekId, vm.NewEntry.SelectedTaskId ?? 0, vm.NewEntry.SelectedJobId ?? 0);
                if(res.Successful)
                {
                    NotificationsController.AddNotification(User.SafeUserName(), "The selected task has been added.");
                }
                else
                {
                    NotificationsController.AddNotification(User.SafeUserName(), "Select task could not be added.");
                }
            }

            if(postType == "Copy Job/Tasks From Previous Week")
            {
                await copyPreviousWeekTimeCommand.CopyPreviousWeekTime(employeeId, weekId);
            }

            if(postType == "Submit")
            {
                var req = new TimeApprovalRequest(
                    approvingUserId: await sessionAdapter.EmployeeIdAsync(),
                    approvingUserIsAdmin: User.IsInRole(UserRoleName.Admin),
                    employeeId: employeeId,
                    newApprovalState: TimeApprovalStatus.Submitted,
                    weekId: weekId
                );
                var res = await approveTimeCommand.ApplyApproval(req);
                NotificationsController.AddNotification(User.SafeUserName(), $"Timesheet is {TimeApprovalStatus.Submitted}");
            }

            if(postType == "Approve")
            {
                var req = new TimeApprovalRequest(
                    approvingUserId: await sessionAdapter.EmployeeIdAsync(),
                    approvingUserIsAdmin: User.IsInRole(UserRoleName.Admin),
                    employeeId: employeeId,
                    newApprovalState: TimeApprovalStatus.Approved,
                    weekId: weekId
                );
                var res = await approveTimeCommand.ApplyApproval(req);
                NotificationsController.AddNotification(User.SafeUserName(), $"Timesheet is {TimeApprovalStatus.Submitted}");
            }

            if(postType == "Reject")
            {
                var req = new TimeApprovalRequest(
                    approvingUserId: await sessionAdapter.EmployeeIdAsync(),
                    approvingUserIsAdmin: User.IsInRole(UserRoleName.Admin),
                    employeeId: employeeId,
                    newApprovalState: TimeApprovalStatus.Rejected,
                    weekId: weekId
                );
                var res = await approveTimeCommand.ApplyApproval(req);
                NotificationsController.AddNotification(User.SafeUserName(), $"Timesheet is {TimeApprovalStatus.Rejected}");

            }
            if(postType == "Save New Combination")
            {
                var rowId = vm.SelectedRowId;
                var oldJobId = int.Parse(rowId.Substring(0, rowId.IndexOf(".")));
                var oldTaskId = int.Parse(rowId.Substring(rowId.IndexOf(".") + 1));
                var res = await modifyJobTaskComboCommand.ModifyJobTaskCombo(employeeId, weekId, vm.NewEntry.SelectedTaskId ?? 0, vm.NewEntry.SelectedJobId ?? 0, oldTaskId, oldJobId);
            }

            if(postType == "RemoveRow")
            {
                var rowId = vm.SelectedRowId;
                var jobId = rowId.Substring(0, rowId.IndexOf("."));
                var taskId = rowId.Substring(rowId.IndexOf(".") + 1);
                var res = await removeRowCommand.RemoveRow(employeeId, weekId, int.Parse(taskId), int.Parse(jobId));
            }
            return RedirectToAction(nameof(Edit), new { weekId = weekId, employeeId = employeeId });
        }
    }

}

