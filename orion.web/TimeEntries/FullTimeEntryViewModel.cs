using System.Collections.Generic;

namespace orion.web.TimeEntries
{
    public class FullTimeEntryViewModel
    {
        public WeekIdentifier Week { get; set; }
        public List<TimeEntryViewModel> TimeEntryRow { get; set; }
        public TimeEntryViewModel NewEntry { get; set; }
        public string NextWeekUrl { get; set; }
        public string PreviousWeekUrl { get; set; }
        public TimeApprovalStatus ApprovalStatus { get; set; }
        public string SelectedRowId { get; set; }
    }
}
