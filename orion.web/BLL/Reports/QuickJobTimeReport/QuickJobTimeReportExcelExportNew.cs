using ClosedXML.Excel;
using orion.web.BLL.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class QuickJobTimeReportExcelExportNew
    {
        public const int EMPLOYEE_ROW_START = 6;

        public MemoryStream AsXls(ReportDTO<QuickJobTimeReportDTO> rpt)
        {
            var workbook = new XLWorkbook("docs/QuickJobTimeReportNew.xlsx", new LoadOptions() { });
            var excelSheet = workbook.Worksheet(1);
            SetHeaderValues(rpt, excelSheet);
            WriteEmployeeRows(rpt, excelSheet);
            WriteExpenseRows(rpt, excelSheet);
            WriteReportMetadata(rpt, excelSheet);
            var ms2 = new MemoryStream();
            workbook.SaveAs(ms2);
            ms2.Position = 0;
            return ms2;
        }

        private void WriteExpenseRows(ReportDTO<QuickJobTimeReportDTO> report, IXLWorksheet excelSheet)
        {
             var rowStart = 8 + report.Data.Employees.Count();
            var originalRowStart = rowStart;
            
            if(report.Data.Expenses.Any())            
                excelSheet.Row(rowStart).InsertRowsBelow(report.Data.Expenses.Count);

            foreach (var exp in report.Data.Expenses)
            {
                excelSheet.Range(rowStart, 1, rowStart, 3).Merge()
                   .AssignValue(exp.Key)
                   .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                   .AddLeftBorder(XLBorderStyleValues.Thin)
                   .AddRightBorder(XLBorderStyleValues.Thin)
                   .SetFontStyle(f => f.Bold = false)
                   .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                   .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Range(rowStart, 4,rowStart, 5).Merge()
                    .AssignValue(exp.Value, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                rowStart++;
            }

              

            if(report.Data.Expenses.Any())
            {
                  string[] columns = new string[] { "turningArrayInto1Based", "A", "B", "C", "D", "E", "F", "G", "H", "I" };
      
                  excelSheet.Range(rowStart, 1, rowStart, 3).Merge()
                      .AddTopBorder(XLBorderStyleValues.Thin)
                      .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                      .AddRightBorder(XLBorderStyleValues.Thin)
                      .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                      .AssignValue("TOTAL")
                      .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                      .SetFontStyle(x => x.Bold = true);

                    var labelTotal = excelSheet.Range(rowStart, 4,rowStart, 5).Merge()
                        .AddRightBorder(XLBorderStyleValues.Thin)
                        .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                        .AddBottomBorder(XLBorderStyleValues.Thin)
                        .AddLeftBorder(XLBorderStyleValues.Thin)
                        .AddTopBorder(XLBorderStyleValues.Thin)
                        .AddRightBorder(XLBorderStyleValues.Thin);
                    labelTotal.SetFontStyle(x => x.Bold = true);
                    labelTotal.FormulaA1 = $"SUM({columns[4]}{originalRowStart}:{columns[4]}{rowStart - 1})";
                    labelTotal.LastCell().CellRight().AddLeftBorder(XLBorderStyleValues.Thin);
            }
        }

        private static void SetHeaderValues(ReportDTO<QuickJobTimeReportDTO> rpt, IXLWorksheet excelSheet)
        {
            excelSheet.Cell(1, 2).Value = (rpt.Data.JobCode);
            excelSheet.Cell(1, 4).Value = (rpt.Data.JobName);

            excelSheet.Cell(2, 2).Value = (rpt.Data.SiteName);
            excelSheet.Cell(2, 5).Value = (rpt.Data.ClientName);

            excelSheet.Cell(3, 5).Value = $"{rpt.Data.PeriodStart.ToShortDateString()} thru {rpt.Data.PeriodEnd.ToShortDateString()}";

        }

        private static void WriteReportMetadata(ReportDTO<QuickJobTimeReportDTO> report, IXLWorksheet excelSheet)
        {
            var row = 10 + report.Data.Employees.Count() + report.Data.Expenses.Count();
            var settingIndex = 0;
            foreach(var item in report.RunSettings)
            {
                excelSheet.Range(row, 1, row, 3).Merge()
                    .AssignValue($"{item.Key}: {item.Value}")
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left);
                row++;
            }
        }

        private static void WriteEmployeeRows(ReportDTO<QuickJobTimeReportDTO> report, IXLWorksheet excelSheet)
        {
            var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            var row = EMPLOYEE_ROW_START;
            if(report.Data.Employees.Any())
            {
                excelSheet.Row(7).InsertRowsAbove(report.Data.Employees.Count());
            }
            foreach(var employeeRow in report.Data.Employees.OrderBy(x => x.TaskCategory).ThenBy(x => x.TaskName).ThenBy(x => x.EmployeeName))
            {
                SetCellContentWithBorder(excelSheet, row, 1, employeeRow.EmployeeName, alignLeft: true, addLeftBorder: true);
                excelSheet.Cell(row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                SetCellContentWithBorder(excelSheet, row, 3, employeeRow.TaskCategory, alignLeft: true);
                SetCellContentWithBorder(excelSheet, row, 4, employeeRow.TaskName, alignLeft: true);
                excelSheet.Cell(row, 5).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                SetCellContentWithBorder(excelSheet, row, 6, (double)employeeRow.Regular);
                SetCellContentWithBorder(excelSheet, row, 7, (double)employeeRow.Overtime);

                excelSheet.Cell(row, 8).SetFormulaA1($"SUM({columns[5]}{row },{columns[6]}{row})");
                excelSheet.Cell(row, 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                row++;
            }

            if(report.Data.Employees.Any())
            {
                excelSheet.Cell(row, 6).SetFormulaA1($"SUM({columns[5]}{EMPLOYEE_ROW_START }:{columns[5]}{row - 1})");
                excelSheet.Cell(row, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 6).Style.Font.Bold = true;

                excelSheet.Cell(row, 7).SetFormulaA1($"SUM({columns[6]}{EMPLOYEE_ROW_START }:{columns[6]}{row - 1})");
                excelSheet.Cell(row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 7).Style.Font.Bold = true;

                excelSheet.Cell(row, 8).SetFormulaA1($"SUM({columns[5]}{row }:{columns[6]}{row})");
                excelSheet.Cell(row, 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelSheet.Cell(row, 8).Style.Font.Bold = true;
            }

        }

        private static void SetCellContentWithBorder(IXLWorksheet excelSheet, int row, int col, object value, bool alignLeft = false, bool addLeftBorder = false, bool addRigtBorder = false)
        {
            if(addLeftBorder)
                excelSheet.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            if(addRigtBorder)
                excelSheet.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;

            if(alignLeft)
                excelSheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            excelSheet.Cell(row, col).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            excelSheet.Cell(row, col).Value = value;
        }
    }
}
