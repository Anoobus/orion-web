using System.Collections.Generic;

namespace orion.web.TimeEntries
{
    public class FullTimeEntryViewModel
    {
        public PageActionType PageActionType { get; set; }
        public WeekOfTimeViewModel Week { get; set; }
        public List<TimeEntryViewModel> AddedEntries { get; set; }
        public TimeEntryViewModel NewEntry { get; set; }
        public string NextWeekUrl { get; set; }
        public string PreviousWeekUrl { get; set; }
    }
}
