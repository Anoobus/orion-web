using orion.web.Jobs;
using orion.web.Reports.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports.QuickJobTimeReport
{

    public class QuickJobTimeReportCriteria
    {
        public const string QUICK_JOB_TIME_REPORT_NAME = "Quick Job Time Report";
        public ReportingPeriod PeriodSettings { get; set; }
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }
}
