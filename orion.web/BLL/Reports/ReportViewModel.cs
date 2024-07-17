using System.Collections.Generic;
using Orion.Web.BLL.Reports.AllOpenJobsSummaryreport;
using Orion.Web.BLL.Reports.DetailedExpenseForJobReport;
using Orion.Web.Reports.Common;
using Orion.Web.Reports.EmployeeTimeReport;
using Orion.Web.Reports.PayPeriodReport;
using Orion.Web.Reports.QuickJobTimeReport;

namespace Orion.Web.Reports
{
    public class ReportSelectionViewModel
    {
        public IEnumerable<string> AvailableReports()
        {
            if (PayPeriodReportCriteria.CanView)
            {
                yield return PayPeriodReportCriteria.ReportName;
            }

            if (QuickJobTimeReportCriteria.CanView)
            {
                yield return QuickJobTimeReportCriteria.ReportName;
            }

            if (EmployeeTimeReportCriteria.CanView)
            {
                yield return EmployeeTimeReportCriteria.ReportName;
            }

            if (DetailedExpenseForJobReportCriteria.CanView)
            {
                yield return DetailedExpenseForJobReportCriteria.ReportName;
            }

            if (AllOpenJobsSummaryReportCriteria.CanView)
            {
                yield return AllOpenJobsSummaryReportCriteria.ReportName;
            }
        }

        public ExcelReport<PayPeriodReportCriteria> PayPeriodReportCriteria { get; set; }
        public ExcelReport<QuickJobTimeReportCriteria> QuickJobTimeReportCriteria { get; set; }
        public ExcelReport<EmployeeTimeReportCriteria> EmployeeTimeReportCriteria { get; set; }
        public ExcelReport<DetailedExpenseForJobReportCriteria> DetailedExpenseForJobReportCriteria { get; set; }
        public ExcelReport<AllOpenJobsSummaryReportCriteria> AllOpenJobsSummaryReportCriteria { get; set; }
    }
}
