using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeApproval;
using System;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{

    [Authorize]
    public class TimeEntryController : Controller
    {
        private const string EDIT_ROUTE = nameof(TimeEntryController) + nameof(Edit);
        private readonly IWeekService weekService;
        private readonly ICopyPreviousWeekTimeCommand copyPreviousWeekTimeCommand;
        private readonly ISaveTimeEntriesCommand saveTimeEntriesCommand;
        private readonly IAddNewJobTaskComboCommand addNewJobTaskComboCommand;
        private readonly IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery;
        private readonly IWeekIdentifierListQuery weekIdentifierListQuery;
        private readonly IApproveTimeCommand approveTimeCommand;
        private readonly IRemoveRowCommand removeRowCommand;

        public TimeEntryController(IWeekService weekService,
            ICopyPreviousWeekTimeCommand copyPreviousWeekTimeCommand,
            ISaveTimeEntriesCommand saveTimeEntriesCommand,
            IAddNewJobTaskComboCommand addNewJobTaskComboCommand,
            IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery,
            IWeekIdentifierListQuery weekIdentifierListQuery,
            IApproveTimeCommand approveTimeCommand,
            IRemoveRowCommand removeRowCommand)
        {
            this.weekService = weekService;
            this.copyPreviousWeekTimeCommand = copyPreviousWeekTimeCommand;
            this.saveTimeEntriesCommand = saveTimeEntriesCommand;
            this.addNewJobTaskComboCommand = addNewJobTaskComboCommand;
            this.weekOfTimeEntriesQuery = weekOfTimeEntriesQuery;
            this.weekIdentifierListQuery = weekIdentifierListQuery;
            this.approveTimeCommand = approveTimeCommand;
            this.removeRowCommand = removeRowCommand;
        }

        public ActionResult Index()
        {
            return View("WeekList", weekIdentifierListQuery.GetWeeks(5));
        }


        public ActionResult Current()
        {
            var current = weekService.Get(DateTime.Now);
            return RedirectToAction("Edit", new { year = current.Year, id = current.WeekId });
        }


        [HttpGet]
        [Route("Edit/year/{year:int=1}/week/{id:int=1}", Name = EDIT_ROUTE)]
        public async Task<ActionResult> Edit(int year, int id)
        {
            var vm = await weekOfTimeEntriesQuery.GetFullTimeEntryViewModel(User.Identity.Name, year, id, EDIT_ROUTE, Url);
            return View("Week", vm);
        }

        [HttpPost]
        [Route("Edit/year/{year:int}/week/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(int year, int id, FullTimeEntryViewModel vm, string postType)
        {
            if(postType == "Save" || postType == "Add Task" || postType == "Submit")
            {
                var res = await saveTimeEntriesCommand.SaveTimeEntriesAsync(User.Identity.Name, year, id, vm);
                if(res.Successful)
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), "Timesheet has been saved");
                }
                else
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), $"Timesheet was not saved {string.Join(",", res.Errors)}");
                }
            }

            if(postType == "Add Task")
            {
                var res = await addNewJobTaskComboCommand.AddNewJobTaskCombo(this.User.Identity.Name, year, id, vm.NewEntry.SelectedTaskId ?? 0, vm.NewEntry.SelectedJobId ?? 0);
                if(res.Successful)
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), "The selected task has been added.");
                }
                else
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), "Select task could not be added.");
                }
            }

            if(postType == "Copy Job/Tasks From Previous Week")
            {
                await copyPreviousWeekTimeCommand.CopyPreviousWeekTime(User.Identity.Name, year, id);
            }

            if(postType == "Submit")
            {
                var res = await approveTimeCommand.ApplyApproval(User.IsInRole(UserRoleName.Admin), User.Identity.Name, new TimeApprovalController.TimeApprovalRequest()
                {
                    AccountName = User.Identity.Name,
                    NewApprovalState = TimeApprovalStatus.Submitted,
                    WeekId = id,
                    Year = year,
                });
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Timesheet is {TimeApprovalStatus.Submitted}");
            }
            if(postType == "RemoveRow")
            {
                var rowId = vm.SelectedRowId;
                var jobId = rowId.Substring(0, rowId.IndexOf("."));
                var taskId = rowId.Substring(rowId.IndexOf(".") + 1);
                var res = await removeRowCommand.RemoveRow(User.Identity.Name, year, id, int.Parse(taskId), int.Parse(jobId));
            }
            return RedirectToAction(nameof(Edit));
        }
    }

}

