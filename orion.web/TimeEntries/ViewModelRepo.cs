using System;
using System.Collections.Generic;
using System.Linq;
using orion.web.Common;
using orion.web.TimeEntries;

namespace orion.web.TimeEntries
{
    public class ViewModelRepo
    {
        private readonly IWeekService weekService;

        public ViewModelRepo(IWeekService weekService)
        {
            this.weekService = weekService;
        }
        public  WeekOfTimeViewModel GetWeekOfTimeViewModel(int year, int id)
        {
            return new WeekOfTimeViewModel()
            {
                WeekEnd = weekService.GetWeekDate(year, id, DayOfWeek.Friday),
                WeekStart = weekService.GetWeekDate(year,id, DayOfWeek.Saturday)
            };
        }
      
        public TimeSpentViewModel EmptyViewModel(DayOfWeek dayOfWeek, int weekid, int year)
        {
            return new TimeSpentViewModel()
            {
                Date = weekService.GetWeekDate(year,weekid, dayOfWeek),
                Hours = 0
            };
        }
        public TimeSpentViewModel MapToViewModel(int weekid, int yearId, DayOfWeek dayOfWeek, IEnumerable<TimeEntryDTO> allEntriesThisWeek)
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
