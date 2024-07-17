using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Orion.Web.BLL.Reports;

namespace Orion.Web.Reports
{
    public class AllOpenJobSummaryReportExcelExport
    {
        public MemoryStream AsXls(ReportDTO<AllOpenJobSummaryReportDTO> rpt)
        {
            var workbook = new XLWorkbook("docs/AllOpenJobsSummaryReportTemplate.xlsx", new LoadOptions() { });

            var excelSheet = workbook.Worksheet(1);

            RemoveEmployeeEntriesWithNoTimePresent(rpt);

            SetHeaderValues(rpt, excelSheet);
            WriteMainJobListSection(rpt, excelSheet);

            WriteReportMetadata(rpt, excelSheet);
            var ms2 = new MemoryStream();
            workbook.SaveAs(ms2);
            ms2.Position = 0;
            return ms2;
        }

        private void RemoveEmployeeEntriesWithNoTimePresent(ReportDTO<AllOpenJobSummaryReportDTO> rpt)
        {
            var toRemove = new List<int>();
            foreach (var emp in rpt.Data.EmployeeIdToNameMap)
            {
                if (!rpt.Data.Rows.Any(x => x.Value.GetTotalHours(emp.Key) > 0))
                {
                    toRemove.Add(emp.Key);
                }
            }

            foreach (var emp in toRemove)
            {
                rpt.Data.EmployeeIdToNameMap.Remove(emp);
            }
        }

        private static void WriteReportMetadata(ReportDTO<AllOpenJobSummaryReportDTO> report, IXLWorksheet excelSheet)
        {
            var rowStart = 6 + report.Data.Rows.Count;

            foreach (var item in report.RunSettings)
            {
                excelSheet.Range(rowStart, 1, rowStart, 4).Merge()
                   .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                   .AssignValue($"{item.Key}: {item.Value}")
                   .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                   .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                   .AddTopBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                   .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);

                rowStart++;
            }
        }

        private void SetHeaderValues(ReportDTO<AllOpenJobSummaryReportDTO> rpt, IXLWorksheet excelSheet)
        {
            // Employee total Hours Header
            // Template contains one cell so we subtract one here to use it
            excelSheet.Cell(1, 2).InsertCellsAfter(rpt.Data.EmployeeIdToNameMap.Count - 1);
            excelSheet.Range(1, 3, 1, 3 + rpt.Data.EmployeeIdToNameMap.Count - 1).Merge()
                .AssignValue("Employee Total Hours")
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetFontStyle(x => x.Bold = true);

            // Employee Name column header
            // Template contains one cell so we subtract one here to use it
            excelSheet.Cell(2, 2).InsertCellsAfter(rpt.Data.EmployeeIdToNameMap.Count - 1);
            var emps = rpt.Data.EmployeeIdToNameMap;
            foreach (var (empInfo, index) in rpt.Data.EmployeeIdToNameMap.OrderBy(x => x.Key)
                                                                         .Select((eInfo, index) => (eInfo, index)))
            {
                excelSheet.Cell(2, index + 3)
                    .AssignValue(empInfo.Value)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .AddTopBorder(XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Medium)
                    .SetFontStyle(x => x.Bold = true);

                excelSheet.Column(index + 3).Width = 16.25;
            }
        }

        private void WriteMainJobListSection(ReportDTO<AllOpenJobSummaryReportDTO> report, IXLWorksheet excelSheet)
        {
            var rowStart = 3;
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.Rows.Count);

            foreach (var row in report.Data.Rows.OrderBy(x => x.Key))
            {
                excelSheet.Cell(rowStart, 1)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AssignValue(row.Key)
                    .AddLeftBorder(XLBorderStyleValues.Medium)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                var expAmount = report.Data.ExpenseAmountForJob.TryGetValue(row.Key, out var temp) ? temp : 0.0m;

                excelSheet.Cell(rowStart, 2)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AssignValue(expAmount, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                foreach (var (empInfo, index) in report.Data.EmployeeIdToNameMap.OrderBy(x => x.Key)
                                                                                .Select((eInfo, index) => (eInfo, index)))
                {
                    excelSheet.Cell(rowStart, 3 + index)
                        .AssignValue(row.Value.GetTotalHours(empInfo.Key))
                        .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                        .AddLeftBorder(XLBorderStyleValues.Thin)
                        .AddRightBorder(XLBorderStyleValues.Dotted)
                        .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                        .AddBottomBorder(XLBorderStyleValues.Thin);
                }

                var labelTotal = excelSheet.Cell(rowStart, 3 + report.Data.EmployeeIdToNameMap.Count)
                                           .AddTopBorder(XLBorderStyleValues.Thin)
                                           .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                                           .AddRightBorder(XLBorderStyleValues.Thin)
                                           .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                                           .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                                           .SetFontStyle(x => x.Bold = true);

                labelTotal.FormulaA1 = $"SUM({XLHelper.GetColumnLetterFromNumber(3)}{rowStart}:{XLHelper.GetColumnLetterFromNumber(3 + report.Data.EmployeeIdToNameMap.Count - 1)}{rowStart})";

                rowStart++;
            }
        }
    }
}
