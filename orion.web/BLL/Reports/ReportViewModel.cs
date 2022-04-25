using orion.web.BLL.Reports.AllOpenJobsSummaryreport;
using orion.web.BLL.Reports.DetailedExpenseForJobReport;
using orion.web.Reports.Common;

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
           
            if(DetailedExpenseForJobReportCriteria.CanView)
            {
                yield return DetailedExpenseForJobReportCriteria.ReportName;
            }
             if(AllOpenJobsSummaryReportCriteria.CanView)
            {
                yield return AllOpenJobsSummaryReportCriteria.ReportName;
            }
        }

        public ExcelReport<PayPeriodReportCriteria> PayPeriodReportCriteria { get; set; }
        public ExcelReport<QuickJobTimeReportCriteria> QuickJobTimeReportCriteria { get; set; }
        
        public ExcelReport<DetailedExpenseForJobReportCriteria> DetailedExpenseForJobReportCriteria { get; set; }
        public ExcelReport<AllOpenJobsSummaryReportCriteria> AllOpenJobsSummaryReportCriteria { get; set; }
    }
}
