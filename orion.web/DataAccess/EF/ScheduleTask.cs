using System;

namespace orion.web.DataAccess.EF
{

    public class ScheduleTask
    {
        public int ScheduleTaskId { get; set; }
        public string TaskName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int RecurEveryNWeeks { get; set; }
        public bool OnMonday { get; set; }
        public bool OnTuesday { get; set; }
        public bool OnWednesday { get; set; }
        public bool OnThursday { get; set; }
        public bool OnFriday { get; set; }
        public bool OnSaturday { get; set; }
        public bool OnSunday { get; set; }
    }
}
