using orion.web.Jobs;
using orion.web.Reports.Common;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.ProjectStatusReport;
using orion.web.Reports.QuickJobTimeReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

            if(ProjectStatusReportCriteria.CanView)
            {
                yield return ProjectStatusReportCriteria.ReportName;
            }
        }
       
        public ExcelReport<PayPeriodReportCriteria> PayPeriodReportCriteria { get; set; }
        public ExcelReport<QuickJobTimeReportCriteria> QuickJobTimeReportCriteria { get; set; }
        public ExcelReport<ProjectStatusReportCriteria> ProjectStatusReportCriteria { get; set; }
    }
}
