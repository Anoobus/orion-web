using System;

namespace Orion.Web.TimeEntries
{
    public class WeekIdentifier
    {
        public const DayOfWeek WEEKSTART = DayOfWeek.Saturday;
        public const DayOfWeek WEEKEND = DayOfWeek.Friday;

        public int WeekId { get; set; }
        public int Year { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
}
