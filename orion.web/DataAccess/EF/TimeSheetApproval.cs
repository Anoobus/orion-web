﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.DataAccess.EF
{
    public class TimeSheetApproval
    {
        public int TimeSheetApprovalId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? ApproverEmployeeId { get; set; }
        public string TimeApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ResponseReason { get; set; }
        public int WeekId { get; set; }
        public bool IsHidden { get; set; }
    }
}
