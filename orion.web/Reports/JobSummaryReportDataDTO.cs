using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class JobSummaryReportDataDTO
    {
        public string JobCode { get; set; }
        public string JobName { get; set; }
        public string SiteName { get; set; }
        public string ClientName { get; set; }        

        public IEnumerable<JobEmployees> Employees { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class JobEmployees
    {
        public string EmployeeName { get; set; }
        public decimal Regular { get; set; }
        public decimal Overtime { get; set; }
        public decimal Combined { get; set; }
        public string TaskCategory { get; set; }
        public string TaskName { get; set; }
    }

}
