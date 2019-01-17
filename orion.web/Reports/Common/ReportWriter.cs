using orion.web.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportWriter : IRegisterByConvention
    {
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<ProjectStatusReportDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt) where T : new();
    }
    public class ReportWriter : IReportWriter
    {
        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt) where T : new()
        {
            var export = new PayPeriodReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt) where T : new()
        {
            var export = new QuickJobTimeReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings,  ReportDTO<ProjectStatusReportDTO> rpt) where T : new()
        {
            var export = new ProjectStatusReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");

        }
    }
}
