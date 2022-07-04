using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class EmployeeTimeReportDTO
    {
        public string EmployeeName { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public IEnumerable<EmployeeTimeEntry> Entries { get; set; }
    }

    public class EmployeeTimeEntry
    {        
        public decimal Regular { get; set; }
        public decimal Overtime { get; set; }        
        public string JobCode { get; set; }
        public string TaskCode { get; set; }
    }

}
