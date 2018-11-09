using System;

namespace orion.web.TimeEntries
{
    public class TimeSummaryDTO
    {
        public int EmployeeId { get; set; }
        public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }
        public int WeekId { get; set; }   
        public int YearId { get; set; }
        public TimeApprovalStatus ApprovalStatus { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
