using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.TimeEntries;

namespace Orion.Web.TimeApproval
{
    public enum ActiveSection
    {
        Submitted = 0, // default to submitted section
        Approved,
        Hidden,
        Rejected,
        Missing
    }

    public class TimeEntriesViewModel
    {
        public IEnumerable<TimeApprovalDTO> Entries { get; set; }
        public bool CurrentUserCanManageTimeApprovals { get; set; }
    }

    public class TimeApprovalList
    {
        public DateTime PeriodStartData { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public TimeEntriesViewModel ApprovedEntries { get; set; }
        public TimeEntriesViewModel SubmittedEntries { get; set; }
        public TimeEntriesViewModel MissingEntries { get; set; }
        public TimeEntriesViewModel RejectedEntries { get; set; }
        public TimeEntriesViewModel HiddenEntries { get; set; }
        public ActiveSection ActiveSection { get; set; }
    }
}
