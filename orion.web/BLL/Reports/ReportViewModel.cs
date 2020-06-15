using orion.web.Reports.Common;
using orion.web.Reports.JobSummaryReport;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.QuickJobTimeReport;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class ReportSelectionViewModel
    {

        public IEnumerable<string> AvailableReports()
        {
            if(PayPeriodReportCriteria.CanView)
            {
                yield return PayPeriodReportCriteria.ReportName;
            }


            if(QuickJobTimeReportCriteria.CanView)
            {
                yield return QuickJobTimeReportCriteria.ReportName;
            }

            if(JobSummaryReportCriteria.CanView)
            {
                yield return JobSummaryReportCriteria.ReportName;
            }
        }

        public ExcelReport<PayPeriodReportCriteria> PayPeriodReportCriteria { get; set; }
        public ExcelReport<QuickJobTimeReportCriteria> QuickJobTimeReportCriteria { get; set; }
        public ExcelReport<JobSummaryReportCriteria> JobSummaryReportCriteria { get; set; }
    }
}
