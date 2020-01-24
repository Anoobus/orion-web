using System;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string ApproverName { get; set; }
        [Display(Name = "Status")]
        public TimeApprovalStatus TimeApprovalStatus { get; set; }
        public int WeekId { get; set; }
        [Display(Name = "Approved On")]
        public DateTime? ApprovalDate { get; set; }
        public string ResponseReason { get; set; }
        [Display(Name = "Submitted On")]
        public DateTime? SubmittedDate { get; set; }
        [Display(Name = "Week Start")]
        public DateTime WeekStartDate { get; set; }
        [Display(Name = "Total Regular")]
        public decimal TotalRegularHours { get; set; }
        [Display(Name ="Total Overtime")]
        public decimal TotalOverTimeHours { get; set; }

        public bool IsHidden { get; set; }
    }
}
