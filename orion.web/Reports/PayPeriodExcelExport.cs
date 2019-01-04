using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class PayPeriodExcelExport
    {
        public MemoryStream AsXls(ReportDTO<PayPeriodDataDTO> rpt)
        {
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = Guid.NewGuid();
            var fName2 = Guid.NewGuid();
            try
            {
                File.Copy("docs/PayPeriodReport.xlsx", $"{fName2}.xlsx");
                using (var fs = new FileStream($"{fName}.xlsx", FileMode.Create))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook($"{fName2}.xlsx");
                    ISheet excelSheet = workbook.GetSheet("PayPeriodReport");

                    var lastRow = 11;


                    var exemptRows = rpt.Data.Employees.Where(x => x.IsExempt).Count();
                    if (exemptRows > 0)
                    {
                        WriteExemptEmployees(rpt, excelSheet, lastRow, exemptRows);
                    }
                    var nonExemptRows = rpt.Data.Employees.Where(x => !x.IsExempt).Count();
                    if (nonExemptRows > 0)
                    {
                        WriteNonExemptEmployees(rpt, excelSheet, lastRow, exemptRows, nonExemptRows);
                    }

                    workbook.Write(fs);
                }
                using (var fs = new FileStream($"{fName}.xlsx", FileMode.Open))

                {
                    fs.CopyTo(ms2);
                }
                ms2.Position = 0;
                return ms2;
            }
            finally
            {
                if (File.Exists($"{fName}.xlsx"))
                {
                    try
                    {
                        File.Delete($"{fName}.xlsx");
                    }
                    catch (Exception)
                    {
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
                        // eat it
                    }
                }
            }

        }

        private static int WriteNonExemptEmployees(ReportDTO<PayPeriodDataDTO> rpt, ISheet excelSheet, int lastRow, int exemptRows, int nonExemptRows)
        {

            int newRows = 0;
            var startingRow = 3 + exemptRows;
             ShiftRowsDown(excelSheet, startingRow, lastRow + exemptRows, nonExemptRows);
            foreach (var employeeRow in rpt.Data.Employees.Where(x => !x.IsExempt))
            {

                var row = excelSheet.CreateRow(startingRow + newRows++);

                row.CreateCell(0).SetCellValue(employeeRow.EmployeeName);
                row.CreateCell(1).SetCellValue(employeeRow.Regular.ToString("N"));
                row.CreateCell(2).SetCellValue(employeeRow.Overtime.ToString("N"));
                row.CreateCell(3).SetCellValue(employeeRow.Vacation.ToString("N"));
                //skip exempt
                row.CreateCell(4);
                row.CreateCell(5).SetCellValue(employeeRow.Sick.ToString("N"));
                row.CreateCell(6).SetCellValue(employeeRow.Personal.ToString("N"));
                row.CreateCell(7).SetCellValue(employeeRow.Holiday.ToString("N"));
                row.CreateCell(8).SetCellValue(employeeRow.ExcusedNoPay.ToString("N"));
                row.CreateCell(9).SetCellValue(employeeRow.Combined.ToString("N"));

            }
            var totalRow = excelSheet.GetRow(startingRow + newRows);
            totalRow.GetCell(1).SetCellFormula("SUM(B4:")
            return newRows;
        }

        private static void ShiftRowsDown(ISheet excelSheet,  int StartWithRow, int lastRow, int shiftSize)
        {
            try
            {
                excelSheet.ShiftRows(StartWithRow, lastRow, shiftSize);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Blew up shifting rows {StartWithRow} to {lastRow} moving to {shiftSize} ... {e}");
                throw;
            }

            //for (int i = StartWithRow; i <= StartWithRow + numberOfRowsToShift; i++)
            //{
            //    try
            //    {
            //            excelSheet.ShiftRows(StartWithRow, StartWithRow + numberOfRowsToShift, shiftSize);
            //        var row = excelSheet.GetRow(i);
            //        if (row != null)
            //        {
            //            row.CopyRowTo(i + shiftSize);
            //        }

            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine($"Blew up with {i} moving to {i + shiftSize} ... {e}");
            //        throw;
            //    }
                
            //}
        }

        private static int WriteExemptEmployees(ReportDTO<PayPeriodDataDTO> rpt, ISheet excelSheet, int lastRow, int exemptRows)
        {
            var newRows = 0;
            var startingRow = 2;
            ShiftRowsDown(excelSheet, startingRow, lastRow, exemptRows);
            foreach (var employeeRow in rpt.Data.Employees.Where(x => x.IsExempt))
            {
                var row = excelSheet.CreateRow(startingRow + newRows++);
                row.CreateCell(0).SetCellValue(employeeRow.EmployeeName);
                row.CreateCell(1).SetCellValue(employeeRow.Regular.ToString("N"));
                row.CreateCell(2).SetCellValue(employeeRow.Overtime.ToString("N"));
                //skip non-exempt
                row.CreateCell(3);
                row.CreateCell(4).SetCellValue(employeeRow.Vacation.ToString("N"));
                row.CreateCell(5).SetCellValue(employeeRow.Sick.ToString("N"));
                row.CreateCell(6).SetCellValue(employeeRow.Personal.ToString("N"));
                row.CreateCell(7).SetCellValue(employeeRow.Holiday.ToString("N"));
                row.CreateCell(8).SetCellValue(employeeRow.ExcusedNoPay.ToString("N"));
                row.CreateCell(9).SetCellValue(employeeRow.Combined.ToString("N"));
            }

            return newRows;
        }
    }
}
