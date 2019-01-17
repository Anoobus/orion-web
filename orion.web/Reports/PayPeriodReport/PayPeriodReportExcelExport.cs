using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class PayPeriodReportExcelExport
    {
        public const int LAST_ROW = 11;
        public const int EXEMPT_START = 3;
        public const int NON_EXEMPT_START = 5;
        public const int GRAND_TOTAL_ROW = 8;

        public MemoryStream AsXls(ReportDTO<PayPeriodReportDTO> rpt)
        {
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = $"fname-1-{Guid.NewGuid()}";
            var fName2 = $"fname-2-{Guid.NewGuid()}";
            File.Copy("docs/PayPeriodReport.xlsx", $"{fName2}.xlsx");

            XSSFWorkbook workbook = new XSSFWorkbook($"{fName2}.xlsx");
            try
            {

                ISheet excelSheet = workbook.GetSheet("PayPeriodReport");

                var exemptRows = rpt.Data.Employees.Where(x => x.IsExempt).Count();
                if (exemptRows > 0)
                {
                    WriteExemptEmployees(rpt, excelSheet, LAST_ROW, exemptRows);
                }
                var nonExemptRows = rpt.Data.Employees.Where(x => !x.IsExempt).Count();
                if (nonExemptRows > 0)
                {
                    WriteNonExemptEmployees(rpt, excelSheet, LAST_ROW, exemptRows, nonExemptRows);
                }

                WriteGrandTotalRow(excelSheet, exemptRows, nonExemptRows);
                WriteReportMetadata(rpt, excelSheet, exemptRows, nonExemptRows, LAST_ROW);

                // these FileStreams can't be combined becuase the workbook.Write closes the stream
                using (var fs = new FileStream($"{fName}.xlsx", FileMode.Create))
                {
                    workbook.Write(fs);
                }
                //so we re-open it so that we can copy it to our memory stream
                using (var fs = new FileStream($"{fName}.xlsx", FileMode.Open))

                {
                    fs.CopyTo(ms2);
                }
                ms2.Position = 0;
                return ms2;
            }
            finally
            {
                try
                {
                    workbook.Close();
                    workbook = null;
                }
                catch (Exception)
                {
                    Console.WriteLine("no");
                    // eat it
                }

                if (File.Exists($"{fName}.xlsx"))
                {
                    try
                    {
                        File.Delete($"{fName}.xlsx");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("no");
                        // eat it
                    }
                }

                if (File.Exists($"{fName2}.xlsx"))
                {
                    try
                    {
                        File.Delete($"{fName2}.xlsx");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("no");
                        // eat it
                    }
                }
            }

        }

        private static void WriteReportMetadata(ReportDTO<PayPeriodReportDTO> report, ISheet excelSheet, int exemptRows, int nonExemptRows, int lastRow)
        {
            var rowCount = 0;
            foreach (var item in report.RunSettings)
            {
                var totalRow = excelSheet.CreateRow(lastRow - 1 + exemptRows + nonExemptRows + rowCount++);
                totalRow.CreateCell(0).SetCellValue($"{item.Key}: {item.Value}");
            }
        }

        private static void WriteGrandTotalRow(ISheet excelSheet, int exemptRows, int nonExemptRows)
        {

            var totalRow = excelSheet.GetRow(GRAND_TOTAL_ROW + exemptRows + nonExemptRows);
            var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            foreach (var totalCell in totalRow.Cells.Skip(1))
            {
                //SUM(B6,B4)
                var exemptCell = $"{columns[totalCell.ColumnIndex]}{EXEMPT_START + exemptRows + 1}";
                var nonExemptCell = $"{columns[totalCell.ColumnIndex]}{NON_EXEMPT_START + exemptRows + nonExemptRows + 1}";
                totalCell.SetCellFormula($"SUM({exemptCell},{nonExemptCell})");
            }
        }

        private static void WriteNonExemptEmployees(ReportDTO<PayPeriodReportDTO> rpt, ISheet excelSheet, int lastRow, int exemptRows, int nonExemptRows)
        {
            int newRows = 0;
            var startingRow = NON_EXEMPT_START + exemptRows;
            excelSheet.ShiftRows(startingRow, lastRow + exemptRows, nonExemptRows);
            foreach (var employeeRow in rpt.Data.Employees.Where(x => !x.IsExempt))
            {
                var row = excelSheet.CreateRow(startingRow + newRows++);
                row.CreateCell(0).SetCellValue(employeeRow.EmployeeName);
                row.CreateCell(1).SetCellValue((double)employeeRow.Regular);
                //skip exempt
                var cell = row.CreateCell(2);
                cell.CellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index;
                row.CreateCell(3).SetCellValue((double)employeeRow.Overtime);
                row.CreateCell(4).SetCellValue((double)employeeRow.PTO);
                row.CreateCell(5).SetCellValue((double)employeeRow.Holiday);
                row.CreateCell(6).SetCellValue((double)employeeRow.ExcusedWithPay);
                row.CreateCell(7).SetCellValue((double)employeeRow.ExcusedNoPay);
                row.CreateCell(8).SetCellValue((double)employeeRow.Combined);               
            }
            PopulateSummaryRow(excelSheet, newRows, startingRow, "C");
        }

        private static void PopulateSummaryRow(ISheet excelSheet, int newRows, int startingRow, string skipCell)
        {
            var totalRow = excelSheet.GetRow(startingRow + newRows);
            var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            foreach (var totalCell in totalRow.Cells.Skip(1))
            {
                if (columns[totalCell.ColumnIndex] != skipCell)
                {
                    totalCell.SetCellFormula($"SUM({columns[totalCell.ColumnIndex]}{startingRow + 1}:{columns[totalCell.ColumnIndex]}{startingRow + newRows })");
                }
            }
        }


        private static void WriteExemptEmployees(ReportDTO<PayPeriodReportDTO> rpt, ISheet excelSheet, int lastRow, int exemptRows)
        {
            var newRows = 0;
            excelSheet.ShiftRows(EXEMPT_START, lastRow, exemptRows);
            foreach (var employeeRow in rpt.Data.Employees.Where(x => x.IsExempt))
            {
                var row = excelSheet.CreateRow(EXEMPT_START + newRows++);
                row.CreateCell(0).SetCellValue(employeeRow.EmployeeName);
                row.CreateCell(1).SetCellValue((double)employeeRow.Regular);
                row.CreateCell(2).SetCellValue((double)employeeRow.Overtime);
                //skip non-exempt
                var cell = row.CreateCell(3);
                cell.CellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index;
                row.CreateCell(4).SetCellValue((double)employeeRow.PTO);
                row.CreateCell(5).SetCellValue((double)employeeRow.Holiday);
                row.CreateCell(6).SetCellValue((double)employeeRow.ExcusedWithPay);
                row.CreateCell(7).SetCellValue((double)employeeRow.ExcusedNoPay);
                row.CreateCell(8).SetCellValue((double)employeeRow.Combined);            
            }
            PopulateSummaryRow(excelSheet, newRows, EXEMPT_START, "D");
        }

       
    }
}
