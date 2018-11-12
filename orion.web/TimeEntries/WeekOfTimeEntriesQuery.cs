using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.JobsTasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface IWeekOfTimeEntriesQuery : IRegisterByConvention
    {
        Task<FullTimeEntryViewModel> GetFullTimeEntryViewModel(string employeeName, int year, int id, string editRouteName, IUrlHelper urlHelper);
    }
    public class WeekOfTimeEntriesQuery : IWeekOfTimeEntriesQuery
    {
        private readonly ITimeService timeService;
        private readonly IJobService jobService;
        private readonly ITaskService taskService;
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IWeekService weekService;
        

        public WeekOfTimeEntriesQuery(ITimeService timeService, 
            IJobService jobService, 
            ITaskService taskService,
            ITimeApprovalService timeApprovalService,
            IWeekService weekService)
        {
            this.timeService = timeService;
            this.jobService = jobService;
            this.taskService = taskService;
            this.timeApprovalService = timeApprovalService;
            this.weekService = weekService;
            
        }

        public async Task<FullTimeEntryViewModel> GetFullTimeEntryViewModel(string employeeName,int year, int id, string editRouteName, IUrlHelper urlHelper)
        {
            Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            var timeEntries = await timeService.GetAsync(year, id, employeeName);
            var jobList = jobService.Get(employeeName).ToList();
            var taskList = taskService.GetTasks().ToList();
            var entries = new List<TimeEntryViewModel>();
            var status = await timeApprovalService.Get(year, id,employeeName);

            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var item = new TimeEntryViewModel()
                {
                    SelectedJobId = entry.Key.JobId,
                    SelectedTaskId = entry.Key.JobTaskId,
                    AvailableJobs = jobList.Where(x => x.JobId == entry.Key.JobId),
                    AvailableTasks = taskList,
                    Monday = MapToViewModel(id, year, DayOfWeek.Monday, entry),
                    Tuesday = MapToViewModel(id, year, DayOfWeek.Tuesday, entry),
                    Wednesday = MapToViewModel(id, year, DayOfWeek.Wednesday, entry),
                    Thursday = MapToViewModel(id, year, DayOfWeek.Thursday, entry),
                    Friday = MapToViewModel(id, year, DayOfWeek.Friday, entry),
                    Saturday = MapToViewModel(id, year, DayOfWeek.Saturday, entry),
                    Sunday = MapToViewModel(id, year, DayOfWeek.Sunday, entry),
                     
                };
                entries.Add(item);
            }

            entries = entries.OrderBy(x => x.SelectedJobCode()).ToList();
            var nextWeek =  weekService.Next(year, id);
            var prevWeek = weekService.Previous(year, id);
            return new FullTimeEntryViewModel()
            {
                TimeEntryRow = entries,
                NewEntry = GenerateEmptyJobTask(year, id, employeeName),
                Week = new WeekIdentifier()
                {
                    WeekEnd = weekService.GetWeekDate(year, id, WeekIdentifier.WEEK_END),
                    WeekStart = weekService.GetWeekDate(year, id, WeekIdentifier.WEEK_START),
                    WeekId = id,
                    Year = year
                },
                NextWeekUrl = urlHelper.RouteUrl(editRouteName, new { year = nextWeek.Year, id = nextWeek.WeekId }) ,
                PreviousWeekUrl = urlHelper.RouteUrl(editRouteName, new { year = prevWeek.Year, id = prevWeek.WeekId }),
                ApprovalStatus = status.TimeApprovalStatus
            };

            
        }

        private TimeEntryViewModel GenerateEmptyJobTask(int year, int id, string employeeName)
        {
            var jobList = jobService.Get(employeeName).ToList();
            var taskList = taskService.GetTasks().ToList();
            return new TimeEntryViewModel()
            {
                SelectedJobId = null,
                SelectedTaskId = null,
                AvailableJobs = jobList,
                AvailableTasks = taskList,
                Monday = EmptyViewModel(DayOfWeek.Monday, id, year),
                Tuesday = EmptyViewModel(DayOfWeek.Tuesday, id, year),
                Wednesday = EmptyViewModel(DayOfWeek.Wednesday, id, year),
                Thursday = EmptyViewModel(DayOfWeek.Thursday, id, year),
                Friday = EmptyViewModel(DayOfWeek.Friday, id, year),
                Saturday = EmptyViewModel(DayOfWeek.Saturday, id, year),
                Sunday = EmptyViewModel(DayOfWeek.Sunday, id, year),
            };
        }

        public TimeSpentViewModel EmptyViewModel(DayOfWeek dayOfWeek, int weekid, int year)
        {
            return new TimeSpentViewModel()
            {
                Date = weekService.GetWeekDate(year, weekid, dayOfWeek),
                Hours = 0
            };
        }
      
        private TimeSpentViewModel MapToViewModel(int weekid, int yearId, DayOfWeek dayOfWeek, IEnumerable<TimeEntryDTO> allEntriesThisWeek)
        {
            var thisDaysEntry = allEntriesThisWeek.SingleOrDefault(x => x.Date.DayOfWeek == dayOfWeek);
            return new TimeSpentViewModel()
            {
                DayOfWeek = dayOfWeek,
                Date = weekService.GetWeekDate(yearId, weekid, dayOfWeek),
                Hours = thisDaysEntry?.Hours ?? 0,
                OvertimeHours = thisDaysEntry?.OvertimeHours ?? 0,
                TimeEntryId = thisDaysEntry?.TimeEntryId ?? 0
            };
        }
       
    }
}
