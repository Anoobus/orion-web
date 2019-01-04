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
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<PayPeriodDataDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<JobSummaryReportDataDTO> rpt) where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<DataTable> rpt) where T : new();
    }
    public class ReportWriter : IReportWriter
    {
        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<PayPeriodDataDTO> rpt) where T : new()
        {
            var export = new PayPeriodExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<DataTable> rpt) where T : new()
        {
            var export = new ExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, string fileName, ReportDTO<JobSummaryReportDataDTO> rpt) where T : new()
        {
            var export = new JobSummaryExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");

        }
    }
}
