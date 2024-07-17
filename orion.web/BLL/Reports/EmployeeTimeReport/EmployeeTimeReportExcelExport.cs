using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Orion.Web.BLL.Reports;

namespace Orion.Web.Reports
{
    public class EmployeeTimeReportExcelExport
    {
        public const int EMPLOYEEROWSTART = 6;

        public MemoryStream AsXls(ReportDTO<EmployeeTimeReportDTO> rpt)
        {
            var workbook = new XLWorkbook("docs/EmployeeTimeReport.xlsx", new LoadOptions() { });
            var excelSheet = workbook.Worksheet(1);
            SetHeaderValues(rpt, excelSheet);
            WriteEmployeeRows(rpt, excelSheet);

            // WriteExpenseRows(rpt, excelSheet);
            // WriteReportMetadata(rpt, excelSheet);
            var ms2 = new MemoryStream();
            workbook.SaveAs(ms2);
            ms2.Position = 0;
            return ms2;
        }

        private void WriteEmployeeRows(ReportDTO<EmployeeTimeReportDTO> rpt, IXLWorksheet excelSheet)
        {
            var rowStart = 5;
            var originalRowStart = rowStart;
            if (rpt.Data.Entries.Any())
            {
                excelSheet.Row(rowStart).InsertRowsBelow(rpt.Data.Entries.Count() + 1);
            }

            foreach (var exp in rpt.Data.Entries)
            {
                excelSheet.Range(rowStart, 1, rowStart, 4).Merge()
                   .AssignValue(exp.JobCode)
                   .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                   .AddLeftBorder(XLBorderStyleValues.Thin)
                   .AddRightBorder(XLBorderStyleValues.Thin)
                   .SetFontStyle(f => f.Bold = false)
                   .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                   .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Range(rowStart, 5, rowStart, 6).Merge()
                    .AssignValue(exp.TaskCode)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 7)
                  .AssignValue(exp.Regular, dataFormatOverride: (XLDataType.Number, "0.0"))
                  .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                  .AddLeftBorder(XLBorderStyleValues.Thin)
                  .AddRightBorder(XLBorderStyleValues.Thin)
                  .SetFontStyle(f => f.Bold = false)
                  .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                  .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 8)
                   .AssignValue(exp.Overtime, dataFormatOverride: (XLDataType.Number, "0.0"))
                   .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                   .AddLeftBorder(XLBorderStyleValues.Thin)
                   .AddRightBorder(XLBorderStyleValues.Thin)
                   .SetFontStyle(f => f.Bold = false)
                   .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                   .AddBottomBorder(XLBorderStyleValues.Thin);

                rowStart++;
            }

            excelSheet.Range(rowStart, 5, rowStart, 6).Merge()
                   .AssignValue("Totals")
                   .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                   .AddLeftBorder(XLBorderStyleValues.Thin)
                   .AddRightBorder(XLBorderStyleValues.Thin)
                   .SetFontStyle(f => f.Bold = false)
                   .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                   .AddBottomBorder(XLBorderStyleValues.Thin);

            excelSheet.Cell(rowStart, 7)
              .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
              .AddLeftBorder(XLBorderStyleValues.Thin)
              .AddRightBorder(XLBorderStyleValues.Thin)
              .SetFontStyle(f => f.Bold = false)
              .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
              .AddBottomBorder(XLBorderStyleValues.Thin)
              .SetFormulaA1($"SUM(G5:G{rowStart - 1})");

            excelSheet.Cell(rowStart, 8)
               .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
               .AddLeftBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin)
               .SetFontStyle(f => f.Bold = false)
               .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
               .AddBottomBorder(XLBorderStyleValues.Thin)
               .SetFormulaA1($"SUM(H5:H{rowStart - 1})");
        }

        private static void SetHeaderValues(ReportDTO<EmployeeTimeReportDTO> rpt, IXLWorksheet excelSheet)
        {
            excelSheet.Cell(1, 4).Value = rpt.Data.EmployeeName;
            excelSheet.Cell(2, 5).Value = $"{rpt.Data.PeriodStart.ToShortDateString()} thru {rpt.Data.PeriodEnd.ToShortDateString()}";
        }

        private static void WriteReportMetadata(ReportDTO<QuickJobTimeReportDTO> report, IXLWorksheet excelSheet)
        {
            var row = 10 + report.Data.Employees.Count() + report.Data.Expenses.Count();

            foreach (var item in report.RunSettings)
            {
                excelSheet.Range(row, 1, row, 3).Merge()
                    .AssignValue($"{item.Key}: {item.Value}")
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left);
                row++;
            }
        }
    }
}
