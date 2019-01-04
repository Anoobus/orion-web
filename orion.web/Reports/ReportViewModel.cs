using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Reports
{

    public class JobSummaryReportSettings 
    {
        [Display(Name ="Show Jobs All Jobs (Even Those With 0 Time Spent)")]
        public bool ShowAllJobsRegardlessOfHoursBooked { get; set; }
        public PeriodBasedReportSettings PeriodSettings { get; set; }
    }
    public class JobDetailReport 
    {
        public PeriodBasedReportSettings PeriodSettings { get; set; }
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }

    public class PayPeriodReport
    {
        [Display(Name = "Pay Period End Date")]
        public DateTime PayPeriodEnd { get; set; }
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
        public IEnumerable<string> AvailableReports { get; set; }
       
        public PayPeriodReport PayPeriodReport { get; set; }
        public JobDetailReport JobDetailReport { get; set; }
        public JobSummaryReportSettings JobSummaryReport { get; set; }
    }

}
