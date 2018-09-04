using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class TimeSpentViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public int TimeEntryId { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }
    }
}
