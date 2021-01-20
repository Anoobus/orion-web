
using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class PayPeriodReportExcelExportNew
    {
        public const int LAST_ROW = 11;
        public const int EXEMPT_START = 3;
        public const int NON_EXEMPT_START = 5;
        public const int GRAND_TOTAL_ROW = 9;


        public const int INITIAL_NON_EXEMPT_TOTAL_ROW = 6;
        public const int INITIAL_EXEMPT_TOTAL_ROW = 4;

        public MemoryStream AsXls(ReportDTO<PayPeriodReportDTO> rpt)
        {
            var workbook = new XLWorkbook("docs/PayPeriodReport.xlsx", new LoadOptions() { });


            var excelSheet = workbook.Worksheet(1);

            WriteExemptEmployees(rpt, excelSheet);
            WriteNonExemptEmployees(rpt, excelSheet);

            var nonExemptRows = rpt.Data.Employees.Where(x => !x.IsExempt).Count();
            var exemptRows = rpt.Data.Employees.Where(x => x.IsExempt).Count();
            WriteGrandTotalRow(excelSheet, exemptRows, nonExemptRows);
            WriteReportMetadata(rpt, excelSheet, exemptRows, nonExemptRows);
            var ms2 = new MemoryStream();
            workbook.SaveAs(ms2);
            ms2.Position = 0;
            return ms2;



        }

        private static void WriteReportMetadata(ReportDTO<PayPeriodReportDTO> report, IXLWorksheet excelSheet, int exemptRows, int nonExemptRows)
        {
            var target = GRAND_TOTAL_ROW + exemptRows + nonExemptRows + 2;
            excelSheet.Row(target).InsertRowsBelow(report.RunSettings.Count);
            foreach(var item in report.RunSettings.Select((e, index) => (index, e)))
            {
                excelSheet.Cell(target + item.index, 1).Value = $"{item.e.Key}: {item.e.Value}";
            }
        }

        private static void WriteGrandTotalRow(IXLWorksheet excelSheet, int exemptRows, int nonExemptRows)
        {

            var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            for(int colIndex = 1; colIndex < columns.Length; colIndex++)
            {
                var exemptCell = $"{columns[colIndex]}{INITIAL_EXEMPT_TOTAL_ROW + exemptRows }";
                var nonExemptCell = $"{columns[colIndex]}{INITIAL_NON_EXEMPT_TOTAL_ROW + exemptRows + nonExemptRows }";
                excelSheet.Cell(GRAND_TOTAL_ROW + exemptRows + nonExemptRows, colIndex + 1).SetFormulaA1($"SUM({exemptCell},{nonExemptCell})");
            }
        }

        private static void WriteNonExemptEmployees(ReportDTO<PayPeriodReportDTO> rpt, IXLWorksheet excelSheet)
        {
            var exemptEmployeeRows = rpt.Data.Employees.Where(x => x.IsExempt).Count();
            var nonExemptEmployees = rpt.Data.Employees.Where(x => !x.IsExempt).ToList();
            var activeRow = INITIAL_NON_EXEMPT_TOTAL_ROW + exemptEmployeeRows;
            if(nonExemptEmployees.Any())
            {
                var row = excelSheet.Row(activeRow).InsertRowsAbove(nonExemptEmployees.Count);
                row.Style.Fill.BackgroundColor = XLColor.NoColor;
                row.Style.Font.Bold = false;
                row.Style.Font.FontColor = XLColor.Black;
                foreach(var employeeRow in nonExemptEmployees.Select((e, index) => (index, e)))
                {
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 1), employeeRow.e.EmployeeName);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 2), (double)employeeRow.e.Regular);

                    excelSheet.Cell(activeRow + employeeRow.index, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(208, 206, 206);

                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 4), (double)employeeRow.e.Overtime);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 5), (double)employeeRow.e.PTO);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 6), (double)employeeRow.e.Holiday);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 7), (double)employeeRow.e.ExcusedWithPay);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 8), (double)employeeRow.e.ExcusedNoPay);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 9), (double)employeeRow.e.Combined);
                }
                PopulateSummaryRow(excelSheet, nonExemptEmployees.Count, activeRow, "C");
            }
        }

        private static void PopulateSummaryRow(IXLWorksheet excelSheet, int insertedRows, int startingRow, string skipCell)
        {
            if(insertedRows > 0)
            {
                var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
                var firstEmpRow = startingRow;
                var lastEmptRow = startingRow + insertedRows - 1;
                for(var col = 1; col < columns.Length; col++)
                {
                    if(columns[col] != skipCell)
                    {
                        excelSheet.Cell(startingRow + insertedRows, col+1).SetFormulaA1($"SUM({columns[col]}{firstEmpRow}:{columns[col]}{lastEmptRow})");
                    }
                }
            }
        }


        private static void WriteExemptEmployees(ReportDTO<PayPeriodReportDTO> rpt, IXLWorksheet excelSheet)
        {
            var exemptEmployees = rpt.Data.Employees.Where(x => x.IsExempt).ToList();
            if(exemptEmployees.Any())
            {
                var activeRow = INITIAL_EXEMPT_TOTAL_ROW;
                var row = excelSheet.Row(INITIAL_EXEMPT_TOTAL_ROW).InsertRowsAbove(exemptEmployees.Count);
                row.Style.Fill.BackgroundColor = XLColor.NoColor;
                row.Style.Font.Bold = false;
                row.Style.Font.FontColor = XLColor.Black;
                foreach(var employeeRow in exemptEmployees.Select((e, index) => (index, e)))
                {

                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 1), employeeRow.e.EmployeeName);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 2), (double)employeeRow.e.Regular);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 3), (double)employeeRow.e.Overtime);
                    excelSheet.Cell(activeRow + employeeRow.index, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(208, 206, 206);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 5), (double)employeeRow.e.PTO);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 6), (double)employeeRow.e.Holiday);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 7), (double)employeeRow.e.ExcusedWithPay);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 8), (double)employeeRow.e.ExcusedNoPay);
                    SetReportCell(excelSheet.Cell(activeRow + employeeRow.index, 9), (double)employeeRow.e.Combined);

                }
                PopulateSummaryRow(excelSheet, exemptEmployees.Count, activeRow, "D");
            }
        }

        private static void SetReportCell(IXLCell cell, object value)
        {
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Value = value;
        }

    }
}
