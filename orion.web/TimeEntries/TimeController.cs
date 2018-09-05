using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using orion.web.Employees;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.JobsTasks;
using orion.web.Notifications;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{

    [Authorize]
    public class TimeController : Controller
    {
        private const string EDIT_ROUTE = nameof(TimeController) +  nameof(Edit);
        private readonly IJobService jobService;
        private readonly ITaskService taskService;
        private readonly ITimeService timeService;
        private readonly IEmployeeService employeeService;
        private readonly IWeekService weekService;
        private readonly ViewModelRepo viewModelRepo;
        private readonly ITimeApprovalService timeApprovalService;

        public TimeController(IJobService jobService, 
            ITaskService taskService, 
            ITimeService timeService, 
            IEmployeeService employeeService,
            IWeekService weekService,
            ViewModelRepo viewModelRepo,
            ITimeApprovalService timeApprovalService)
        {
            this.jobService = jobService;
            this.taskService = taskService;
            this.timeService = timeService;
            this.employeeService = employeeService;
            this.weekService = weekService;
            this.viewModelRepo = viewModelRepo;
            this.timeApprovalService = timeApprovalService;
        }
        public ActionResult Index()
        {
            //show 3 weeks back + current
            var dt = DateTime.Now;
            var thisWeek = weekService.Get(dt).WeekId;

            var temp = new List<WeekTimeEntry>();
            for(int i = thisWeek; i > (thisWeek - 3); i--)
            {
                var vm = viewModelRepo.GetWeekOfTimeViewModel(dt.Year, i);
                temp.Add(new WeekTimeEntry()
                {
                    WeekEnd = vm.WeekEnd,
                    WeekStart = vm.WeekStart,
                    WeekId = i
                });
            }

            return View("Index", temp);
        }


        public ActionResult Current()
        {
            var current = weekService.Get(DateTime.Now);
            return RedirectToAction("Edit", new { year = current.Year, id = current.WeekId });
        }

        
        [HttpGet]
        [Route("Edit/year/{year:int=1}/week/{id:int=1}", Name = EDIT_ROUTE)]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int year, int id)
        {
            Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            var employeeName = this.User.Identity.Name;
            var timeEntries = timeService.Get(year,id, employeeName);
            var jobList = jobService.Get(employeeName).ToList();
            var taskList = taskService.GetTasks().ToList();
            var entries = new List<TimeEntryViewModel>();
            var status = await timeApprovalService.Get(year, id, User.Identity.Name);
            Enum.TryParse<TimeApprovalStatus>(status.TimeApprovalStatus, out var mappedStatus);
            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var item = new TimeEntryViewModel()
                {
                    SelectedJobId = entry.Key.JobId,
                    SelectedTaskId = entry.Key.JobTaskId,
                    AvailableJobs = jobList.Where(x => x.JobId == entry.Key.JobId),
                    AvailableTasks = taskList.GroupBy(x => x.TaskCategoryName).ToDictionary(x => x.Key, x => x.AsEnumerable()),
                    Monday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Monday, entry),
                    Tuesday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Tuesday, entry),
                    Wednesday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Wednesday, entry),
                    Thursday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Thursday, entry),
                    Friday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Friday, entry),
                    Saturday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Saturday, entry),
                    Sunday = viewModelRepo.MapToViewModel(id, year, DayOfWeek.Sunday, entry),
                    ApprovalStatus = mappedStatus
                };
                entries.Add(item);
            }

            var vm = new FullTimeEntryViewModel()
            {
                AddedEntries = entries,
                NewEntry = GenerateEmptyJobTask(year, id),
                Week = viewModelRepo.GetWeekOfTimeViewModel(year, id),
                NextWeekUrl = GetUrl(weekService.Next(year, id)),
                PreviousWeekUrl = GetUrl(weekService.Previous( year, id)),

            };

            return View("EditNew", vm);
        }

        private string GetUrl(WeekDTO weekDTO)
        {
            return this.Url.RouteUrl(EDIT_ROUTE, new { year = weekDTO.Year, id = weekDTO.WeekId});
        }

        private TimeEntryViewModel GenerateEmptyJobTask(int year, int id)
        {
            var jobList = jobService.Get(this.User.Identity.Name).ToList();
            var taskList = taskService.GetTasks().ToList();
            return new TimeEntryViewModel()
            {
                SelectedJobId = null,
                SelectedTaskId = null,
                AvailableJobs = jobList,
                AvailableTasks = taskList.GroupBy(x => x.TaskCategoryName).ToDictionary(x => x.Key, x => x.AsEnumerable()),
                Monday = viewModelRepo.EmptyViewModel(DayOfWeek.Monday, id, year),
                Tuesday = viewModelRepo.EmptyViewModel(DayOfWeek.Tuesday, id, year),
                Wednesday = viewModelRepo.EmptyViewModel(DayOfWeek.Wednesday, id, year),
                Thursday = viewModelRepo.EmptyViewModel(DayOfWeek.Thursday, id, year),
                Friday = viewModelRepo.EmptyViewModel(DayOfWeek.Friday, id, year),
                Saturday = viewModelRepo.EmptyViewModel(DayOfWeek.Saturday, id, year),
                Sunday = viewModelRepo.EmptyViewModel(DayOfWeek.Sunday, id, year),
            };
        }

        [HttpPost]
        [Route("Edit/year/{year:int}/week/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(int year, int id, FullTimeEntryViewModel vm)
        {
            await SaveTimeEntriesAsync(year, id, vm);            
            return RedirectToAction(nameof(Edit));
        }

        private async Task SaveTimeEntriesAsync(int year, int id, FullTimeEntryViewModel vm)
        {
            var status = await timeApprovalService.Get(year, id, User.Identity.Name);
            Enum.TryParse<TimeApprovalStatus>(status.TimeApprovalStatus, out var mappedStatus);
            if(mappedStatus != TimeApprovalStatus.Submitted && mappedStatus != TimeApprovalStatus.Approved)
            {


                var employeeName = this.User.Identity.Name;
                var timeEntries = timeService.Get(year, id, employeeName);
                if(vm.AddedEntries != null)
                {
                    foreach(var item in vm.AddedEntries)
                    {
                        foreach(var day in item.AllDays())
                        {
                            var match = timeEntries.FirstOrDefault(x => x.TimeEntryId == day.TimeEntryId);
                            match.Hours = day.Hours;
                            match.OvertimeHours = day.OvertimeHours;
                            timeService.Save(year, id, employeeName, match);
                        }
                    }
                }
                if(vm.PageActionType == PageActionType.Addtask)
                {
                    var thingsToAdd = GenerateEmptyJobTask(year, id);
                    var empoyId = employeeService.GetSingleEmployee(employeeName).EmployeeId;
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Monday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Tuesday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Wednesday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Thursday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Friday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Saturday));
                    timeService.Save(year, id, employeeName, CreateDTO(id, vm, timeService, empoyId, thingsToAdd, thingsToAdd.Sunday));
                }

                NotificationsController.AddNotification(this.User.SafeUserName(), "Timesheet has been saved");
            }
            else
            {
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Timesheet was not saved becuase it is currently {status}");
            }
        }

        [HttpGet]
        [Route("Edit/year/{year:int}/week/{id:int}/SubmitForApproval")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitForApproval(int year, int id)
        {
            await timeApprovalService.Save(new TimeApprovalDTO()
            {
                ApprovalDate = DateTime.Now,
                EmployeeName = this.User.Identity.Name,
                TimeApprovalStatus = TimeApprovalStatus.Submitted.ToString(),
                WeekId = id,
                Year = year
            });
            NotificationsController.AddNotification(this.User.SafeUserName(), "Timesheet is submitted");
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        [Route("Edit/year/{year:int}/week/{id:int}/Employee/{employeeName}/Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleName.Admin)]
        public async Task<ActionResult> ApproveAsync(int year, int id, string employeeName)
        {
            await timeApprovalService.Save(new TimeApprovalDTO()
            {
                ApprovalDate = DateTime.Now,
                ApproverName = this.User.Identity.Name,
                EmployeeName = employeeName,
                TimeApprovalStatus = TimeApprovalStatus.Approved.ToString(),
                WeekId = id,
                Year = year
            });
            NotificationsController.AddNotification(this.User.SafeUserName(), "Timesheet has been approved");
            return RedirectToAction("Index", "Employee");
        }

        private static TimeEntryDTO CreateDTO(int id, FullTimeEntryViewModel vm, ITimeService thing, int employeeId, TimeEntryViewModel thingsToAdd, TimeSpentViewModel marker)
        {
            thingsToAdd.SelectedTaskId = vm.NewEntry.SelectedTaskId;
            thingsToAdd.SelectedJobId = vm.NewEntry.SelectedJobId;
            var dto = new TimeEntryDTO()
            {
                Date = marker.Date,
                EmployeeId = employeeId,
                Hours = marker.Hours,
                OvertimeHours = marker.OvertimeHours,
                JobId = thingsToAdd.SelectedJobId.Value,
                JobTaskId = thingsToAdd.SelectedTaskId.Value,
                TimeEntryId = 0,
                WeekId = id
            };
            return dto;
        }
    }
}

