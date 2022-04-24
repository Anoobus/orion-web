using orion.web.Jobs;
using orion.web.Reports.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports.JobSummaryReport
{
    public class JobSummaryReportCriteria
    {

        public const string PROJECT_STATUS_REPORT_NAME = "Project Status Report";

        [Display(Name = "Show Jobs All Jobs (Even Those With 0 Time Spent)")]
        public bool ShowAllJobsRegardlessOfHoursBooked { get; set; }

        public ReportingPeriod PeriodSettings { get; set; }
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }
}
