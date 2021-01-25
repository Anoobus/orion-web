using orion.web.Common;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportWriter
    {
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<JobSummaryReportDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt) where T : new();
    }
    public class ReportWriter : IReportWriter, IAutoRegisterAsSingleton
    {
        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt) where T : new()
        {
            var export = new PayPeriodReportExcelExportNew();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt) where T : new()
        {
            var export = new QuickJobTimeReportExcelExportNew();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings,  ReportDTO<JobSummaryReportDTO> rpt) where T : new()
        {
            var export = new JobSummaryReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");

        }
    }
}
