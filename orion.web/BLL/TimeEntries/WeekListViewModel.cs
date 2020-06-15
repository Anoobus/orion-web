using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class WeekListViewModel
    {
        public IEnumerable<DetailedWeekIdentifier> Weeks { get; set; }
        public int EmployeeId { get; set; }
        public int WeeksToShow { get; set; }
        public DateTime StartWithDate { get; set; }
    }
    public class DetailedWeekIdentifier : WeekIdentifier
    {
        public decimal TotalOverTime { get; set; }
        public decimal TotalRegular { get; set; }
        public bool IsCurrentWeek { get; set; }
        public TimeApprovalStatus ApprovalStatus { get; set; }
    }
}
