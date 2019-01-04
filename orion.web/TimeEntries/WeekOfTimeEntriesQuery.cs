using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.JobsTasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class WeekOfTimeEntriesRequest
    {
        public int EmployeeId { get; set; }
        public int WeekId { get; set; }
        public string RequestingUserName { get; set; }
        public bool RequestingUserIsAdmin { get; set; }
    }

    public interface IWeekOfTimeEntriesQuery : IRegisterByConvention
    {
        Task<FullTimeEntryViewModel> GetFullTimeEntryViewModelAsync(WeekOfTimeEntriesRequest request);
    }
    public class WeekOfTimeEntriesQuery : IWeekOfTimeEntriesQuery
    {
        private readonly ITimeService timeService;
        private readonly IJobService jobService;
        private readonly ITaskService taskService;
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IEmployeeService employeeService;

        public WeekOfTimeEntriesQuery(ITimeService timeService, 
            IJobService jobService, 
            ITaskService taskService,
            ITimeApprovalService timeApprovalService,
            IEmployeeService employeeService
            )
        {
            this.timeService = timeService;
            this.jobService = jobService;
            this.taskService = taskService;
            this.timeApprovalService = timeApprovalService;
            this.employeeService = employeeService;
        }

        public async Task<FullTimeEntryViewModel> GetFullTimeEntryViewModelAsync(WeekOfTimeEntriesRequest request)
        {
            
            var employeeForWeek = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
            if(!request.RequestingUserIsAdmin && employeeForWeek.UserName != request.RequestingUserName)
            {
                throw new UnauthorizedAccessException($"You are not allowed to view others time sheets");
            }

            var currentWeek = WeekDTO.CreateForWeekId(request.WeekId);

            var timeEntries = await timeService.GetAsync(request.WeekId, request.EmployeeId);
            var jobList = (await jobService.GetAsync(request.EmployeeId)).ToList();
            var taskList = taskService.GetTasks().ToList();
            var entries = new List<TimeEntryViewModel>();
            var status = await timeApprovalService.GetAsync(request.WeekId,request.EmployeeId);

            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var item = new TimeEntryViewModel()
                {
                    SelectedJobId = entry.Key.JobId,
                    SelectedTaskId = entry.Key.JobTaskId,
                    AvailableJobs = jobList.Where(x => x.JobId == entry.Key.JobId),
                    AvailableTasks = taskList,
                    Monday = MapToViewModel(currentWeek,  DayOfWeek.Monday, entry),
                    Tuesday = MapToViewModel(currentWeek,  DayOfWeek.Tuesday, entry),
                    Wednesday = MapToViewModel(currentWeek,  DayOfWeek.Wednesday, entry),
                    Thursday = MapToViewModel(currentWeek,  DayOfWeek.Thursday, entry),
                    Friday = MapToViewModel(currentWeek,  DayOfWeek.Friday, entry),
                    Saturday = MapToViewModel(currentWeek,  DayOfWeek.Saturday, entry),
                    Sunday = MapToViewModel(currentWeek,  DayOfWeek.Sunday, entry),
                     
                };
                entries.Add(item);
            }

            entries = entries.OrderBy(x => x.SelectedJobCode()).ToList();
            var nextWeek = currentWeek.Next();
            var prevWeek = currentWeek.Previous();
            return new FullTimeEntryViewModel()
            {
                TimeEntryRow = entries,
                EmployeeId = employeeForWeek.EmployeeId,
                NewEntry = await GenerateEmptyJobTaskAsync( request.WeekId, request.EmployeeId),
                Week = new WeekIdentifier()
                {
                    WeekEnd = currentWeek.WeekEnd,
                    WeekStart = currentWeek.WeekStart,
                    WeekId = currentWeek.WeekId.Value,
                    Year = currentWeek.Year
                },
                //NextWeekUrl = urlHelper.RouteUrl(editRouteName, new { id = nextWeek.WeekId.Value }) ,
                //PreviousWeekUrl = urlHelper.RouteUrl(editRouteName, new {  id = prevWeek.WeekId.Value }),
                ApprovalStatus = status.TimeApprovalStatus
            };

            
        }

        private async Task<TimeEntryViewModel> GenerateEmptyJobTaskAsync(int weekId, int employeeId)
        {
            var jobList = (await jobService.GetAsync(employeeId)).ToList();
            var taskList = taskService.GetTasks().ToList();
            var week = WeekDTO.CreateForWeekId(weekId);
            return new TimeEntryViewModel()
            {
                SelectedJobId = null,
                SelectedTaskId = null,
                AvailableJobs = jobList,
                AvailableTasks = taskList,
                Monday = EmptyViewModel(DayOfWeek.Monday, week),
                Tuesday = EmptyViewModel(DayOfWeek.Tuesday, week),
                Wednesday = EmptyViewModel(DayOfWeek.Wednesday, week),
                Thursday = EmptyViewModel(DayOfWeek.Thursday, week),
                Friday = EmptyViewModel(DayOfWeek.Friday, week),
                Saturday = EmptyViewModel(DayOfWeek.Saturday, week),
                Sunday = EmptyViewModel(DayOfWeek.Sunday, week),
            };
        }

        public TimeSpentViewModel EmptyViewModel(DayOfWeek dayOfWeek, WeekDTO week)
        {
            return new TimeSpentViewModel()
            {
                Date = GetDateFor(week,dayOfWeek),
                Hours = 0
            };
        }
      
        private DateTime GetDateFor(WeekDTO week, DayOfWeek dayOfWeek)
        {
            
            var candidateDay = week.WeekStart;
            while (candidateDay.DayOfWeek != dayOfWeek)
            {
                candidateDay = candidateDay.AddDays(1);
            }
            return candidateDay;
        }
        private TimeSpentViewModel MapToViewModel(WeekDTO week,  DayOfWeek dayOfWeek, IEnumerable<TimeEntryDTO> allEntriesThisWeek)
        {
            var thisDaysEntry = allEntriesThisWeek.SingleOrDefault(x => x.Date.DayOfWeek == dayOfWeek);
            var date = GetDateFor(week, dayOfWeek);
            return new TimeSpentViewModel()
            {
                DayOfWeek = dayOfWeek,
                Date = date,
                Hours = thisDaysEntry?.Hours ?? 0,
                OvertimeHours = thisDaysEntry?.OvertimeHours ?? 0,
                TimeEntryId = thisDaysEntry?.TimeEntryId ?? 0
            };
        }
       
    }
}
