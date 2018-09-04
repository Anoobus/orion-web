using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.DataAccess.EF
{

    public class TimeSheetApproval
    {
        public int TimeSheetApprovalId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? ApproverEmployeeId { get; set; }
        public string TimeApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ResponseReason { get; set; }
        public int Year { get; set; }
        public int WeekId { get; set; }
    }
}
