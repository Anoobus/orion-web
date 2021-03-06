﻿using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class JobSummaryReportExcelExport
    {
        public const int LAST_ROW = 6;
        public const int EMPLOYEE_ROW_START = 5;

        public MemoryStream AsXls(ReportDTO<JobSummaryReportDTO> rpt)
        {
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = $"fname-1-{Guid.NewGuid()}";
            var fName2 = $"fname-2-{Guid.NewGuid()}";
            File.Copy("docs/ProjectStatusReport.xlsx", $"{fName2}.xlsx");

            XSSFWorkbook workbook = new XSSFWorkbook($"{fName2}.xlsx");
            try
            {

                ISheet excelSheet = workbook.GetSheet("ProjectStatusReport");
                SetHeaderValues(rpt, excelSheet);

                var totalRecs = 0;
                foreach (var category in rpt.Data.Employees.GroupBy(x => x.TaskCategory))
                {
                    totalRecs += WriteExemptEmployees(category, excelSheet, LAST_ROW + totalRecs, EMPLOYEE_ROW_START + totalRecs);
                }

                //WriteGrandTotalRow(excelSheet, exemptRows, nonExemptRows);
                WriteReportMetadata(rpt, excelSheet, totalRecs + LAST_ROW);

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

        private static void SetHeaderValues(ReportDTO<JobSummaryReportDTO> rpt, ISheet excelSheet)
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

        private static void WriteReportMetadata(ReportDTO<JobSummaryReportDTO> report, ISheet excelSheet,  int lastRow)
        {
            var rowCount = 0;
            foreach (var item in report.RunSettings)
            {
                var totalRow = excelSheet.CreateRow(lastRow + 1 +  rowCount++);
                totalRow.CreateCell(0).SetCellValue($"{item.Key}: {item.Value}");
            }
        }

        //private static void WriteGrandTotalRow(ISheet excelSheet, int exemptRows, int nonExemptRows)
        //{

        //    var totalRow = excelSheet.GetRow(GRAND_TOTAL_ROW + exemptRows + nonExemptRows);
        //    var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
        //    foreach (var totalCell in totalRow.Cells.Skip(1))
        //    {
        //        //SUM(B6,B4)
        //        var exemptCell = $"{columns[totalCell.ColumnIndex]}{EXEMPT_START + exemptRows + 1}";
        //        var nonExemptCell = $"{columns[totalCell.ColumnIndex]}{NON_EXEMPT_START + exemptRows + nonExemptRows + 1}";
        //        totalCell.SetCellFormula($"SUM({exemptCell},{nonExemptCell})");
        //    }
        //}

        //private static void PopulateSummaryRow(ISheet excelSheet, int newRows, int startingRow, string skipCell)
        //{
        //    var totalRow = excelSheet.GetRow(startingRow + newRows);
        //    var columns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
        //    foreach (var totalCell in totalRow.Cells.Skip(1))
        //    {
        //        if (columns[totalCell.ColumnIndex] != skipCell)
        //        {
        //            totalCell.SetCellFormula($"SUM({columns[totalCell.ColumnIndex]}{startingRow + 1}:{columns[totalCell.ColumnIndex]}{startingRow + newRows })");
        //        }
        //    }
        //}


        private static int WriteExemptEmployees(IGrouping<string,JobEmployees> group, ISheet excelSheet, int lastRow, int startRow)
        {
            var newRows = 1;
            excelSheet.ShiftRows(startRow, startRow, group.Count() + 1);
            var taskRow = excelSheet.CreateRow(startRow);
            taskRow.CreateCell(0).SetCellValue(group.Key);
            foreach (var employeeRow in group)
            {
                var row = excelSheet.CreateRow(startRow + newRows++);
                row.CreateCell(0);
                row.CreateCell(1).SetCellValue(employeeRow.TaskName);
                row.CreateCell(2).SetCellValue(employeeRow.EmployeeName);
                row.CreateCell(3).SetCellValue((double)employeeRow.Regular);
                row.CreateCell(4).SetCellValue((double)employeeRow.Overtime);
                row.CreateCell(5).SetCellValue((double)employeeRow.Combined);
            }
            return newRows;
        }


    }
}
