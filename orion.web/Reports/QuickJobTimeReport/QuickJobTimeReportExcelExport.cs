using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class QuickJobTimeReportExcelExport
    {
       
        public const int EMPLOYEE_ROW_START = 4;

        public MemoryStream AsXls(ReportDTO<QuickJobTimeReportDTO> rpt)
        {
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = $"fname-1-{Guid.NewGuid()}";
            var fName2 = $"fname-2-{Guid.NewGuid()}";
            File.Copy("docs/QuickJobTimeReport.xlsx", $"{fName2}.xlsx");

            XSSFWorkbook workbook = new XSSFWorkbook($"{fName2}.xlsx");
            try
            {

                ISheet excelSheet = workbook.GetSheet("QuickJobTimeReport");
                SetHeaderValues(rpt, excelSheet);

                var totalRecs = WriteExemptEmployees(rpt.Data.Employees,excelSheet, EMPLOYEE_ROW_START);

                //WriteGrandTotalRow(excelSheet, exemptRows, nonExemptRows);
                WriteReportMetadata(rpt, excelSheet, totalRecs + EMPLOYEE_ROW_START + 2);

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

        private static void SetHeaderValues(ReportDTO<QuickJobTimeReportDTO> rpt, ISheet excelSheet)
        {
            var row1 = excelSheet.GetRow(0);
            row1.GetCell(1).SetCellValue(rpt.Data.JobCode);
            row1.GetCell(2).SetCellValue(rpt.Data.JobName);
            var row2 = excelSheet.GetRow(1);
            row2.GetCell(1).SetCellValue(rpt.Data.SiteName);
            row2.GetCell(3).SetCellValue(rpt.Data.ClientName);
            var row3 = excelSheet.GetRow(2);
            row3.GetCell(3).SetCellValue($"{rpt.Data.PeriodStart.ToShortDateString()} thru {rpt.Data.PeriodEnd.ToShortDateString()}");
        }

        private static void WriteReportMetadata(ReportDTO<QuickJobTimeReportDTO> report, ISheet excelSheet,  int lastRow)
        {
            var rowCount = 0;
            foreach (var item in report.RunSettings)
            {
                var totalRow = excelSheet.CreateRow(lastRow + 1 +  rowCount++);
                totalRow.CreateCell(0).SetCellValue($"{item.Key}: {item.Value}");
            }
        }

        private static int WriteExemptEmployees(IEnumerable<QuickJobEmployees> employees, ISheet excelSheet, int startRow)
        {
            var newRows = 1;
            //excelSheet.ShiftRows(startRow, startRow, employees.Count() + 1);
            foreach (var employeeRow in employees)
            {
                var row = excelSheet.CreateRow(startRow + newRows++);
                row.CreateCell(0);
                row.CreateCell(1).SetCellValue(employeeRow.TaskCategory);
                row.CreateCell(2).SetCellValue(employeeRow.TaskName);
                //skip 3
                row.CreateCell(4).SetCellValue((double)employeeRow.Regular);
                row.CreateCell(5).SetCellValue((double)employeeRow.Overtime);
            }
            return newRows;
        }
    }
}
