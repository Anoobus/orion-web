using System;

namespace orion.web.TimeEntries
{
    public enum TimeApprovalStatus
    {
        Unkown,        
        Submitted,
        Rejected,
        Approved
    }
    public class TimeApprovalDTO
    {
        public string EmployeeName { get; set; }
        public string ApproverName { get; set; }
        public string TimeApprovalStatus { get; set; }
        public int Year { get; set; }
        public int WeekId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ResponseReason { get; set; }
    }
}
