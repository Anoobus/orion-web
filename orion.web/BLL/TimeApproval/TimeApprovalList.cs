using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeApproval
{
    public enum ActiveSection
    {
        Submitted = 0, //default to submitted section
        Approved,
        Hidden,
        Rejected,
        Missing
    }
    public class TimeApprovalList
    {
        public DateTime PeriodStartData { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public IEnumerable<TimeApprovalDTO> ApprovedEntries { get; set; }
        public IEnumerable<TimeApprovalDTO> SubmittedEntries { get; set; }
        public IEnumerable<TimeApprovalDTO> MissingEntries { get; set; }
        public IEnumerable<TimeApprovalDTO> RejectedEntries { get; set; }
        public IEnumerable<TimeApprovalDTO> HiddenEntries { get; set; }
        public ActiveSection ActiveSection { get; set; }
    }
}
