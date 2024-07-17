using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Common;
using Orion.Web.Util.IoC;

namespace Orion.Web.Reports
{
    public interface IReportWriter
    {
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt)
            where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult(ReportDTO<AllOpenJobSummaryReportDTO> rpt);
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt)
            where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<DetailedExpenseForJobReportDTO> rpt)
            where T : new();
        (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<EmployeeTimeReportDTO> rpt)
            where T : new();
    }

    public class ReportWriter : IReportWriter, IAutoRegisterAsSingleton
    {
        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<PayPeriodReportDTO> rpt)
            where T : new()
        {
            var export = new PayPeriodReportExcelExportNew();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<QuickJobTimeReportDTO> rpt)
            where T : new()
        {
            var export = new QuickJobTimeReportExcelExportNew();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult(ReportDTO<AllOpenJobSummaryReportDTO> rpt)
        {
            var export = new AllOpenJobSummaryReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<DetailedExpenseForJobReportDTO> rpt)
            where T : new()
        {
            var export = new DetailedExpenseForJobReportExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }

        public (MemoryStream Steam, string MimeType, string Name) GetFinishedResult<T>(T reportSettings, ReportDTO<EmployeeTimeReportDTO> rpt)
            where T : new()
        {
            var export = new EmployeeTimeReportExcelExport();
            var memoryStream = export.AsXls(rpt);
            return (memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{rpt.ReportName}.xlsx");
        }
    }
}
