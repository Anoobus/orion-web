using System;

namespace orion.web.TimeEntries
{
    public class WeekIdentifier
    {
        public const DayOfWeek WEEK_START = DayOfWeek.Saturday;
        public const DayOfWeek WEEK_END = DayOfWeek.Friday;

        public  int WeekId { get; set; }
        public int Year { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
}
