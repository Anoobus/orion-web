using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Reports
{

    public class ReportNameViewModel
    {
        public string ReportSystemName { get; set; }
        public string ReportDisplayName { get; set; }
    }
    public class ReportViewModel<TReportVarient> where TReportVarient : new()
    {
        public ReportViewModel()
        {
            Report = new TReportVarient();
        }
        public TReportVarient Report { get; set; }

        [Display(Name ="Report Name")]
        public ReportNameViewModel SelectedReport { get; set; }

        [Display(Name ="Download As XLS?")]
        public bool ForDownload { get; set; }
       
    }
    public class JobSummaryReportSettings 
    {
        [Display(Name ="Show Jobs All Jobs (Even Those With 0 Time Spent)")]
        public bool ShowAllJobsRegardlessOfHoursBooked { get; set; }
        public PeriodBasedReportSettings PeriodSettings { get; set; }
    }
    public class JobDetailReport 
    {
        public PeriodBasedReportSettings PeriodSettings { get; set; }
        public JobBasedReportSettings JobBasedReportSettings { get; set; }
    }

    public class PayPeriodReport
    {
        public DateTime PayPeriodEnd { get; set; }
    }

    public class JobBasedReportSettings
    {
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }
    public class PeriodBasedReportSettings
    {
        [Display(Name ="Report Start Date")]
        public DateTime Start { get; set; }
        [Display(Name = "Report End Date")]
        public DateTime End { get; set; }
    }
    public class ReportSelectionViewModel
    {
        public IEnumerable<ReportNameViewModel> AvailableReports { get; set; }
        public string SelectedReportSystemName { get; set; }

        public Dictionary<string,string> GetRouteForReport(ReportNameViewModel rpt)
        {
            return new Dictionary<string, string>()
            {
                {"ReportName",  rpt.ReportSystemName}
            };
        }

    }

}
