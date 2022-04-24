using System.IO;
using System.Linq;
using ClosedXML.Excel;
using orion.web.BLL.Reports;
using orion.web.Common;

namespace orion.web.Reports
{
    public class DetailedExpenseForJobReportExport
    {
        public const int LAST_ROW = 11;
        public const int EXEMPT_START = 3;
        public const int NON_EXEMPT_START = 5;
        public const int GRAND_TOTAL_ROW = 9;


        public const int INITIAL_NON_EXEMPT_TOTAL_ROW = 6;
        public const int INITIAL_EXEMPT_TOTAL_ROW = 4;
        private static readonly string[] columns = new string[] { "turningArrayInto1Based", "A", "B", "C", "D", "E", "F", "G", "H", "I" };
        public MemoryStream AsXls(ReportDTO<DetailedExpenseForJobReportDTO> rpt)
        {
            var workbook = new XLWorkbook("docs/DetailedExpenseReportTemplate.xlsx", new LoadOptions() { });


            var excelSheet = workbook.Worksheet(1);


            SetHeaderValues(rpt, excelSheet);
            WriteTimeAndExpensesSection(rpt, excelSheet);
            WriteCompanyVehicleSection(rpt, excelSheet);
            WriteSubContractorSection(rpt, excelSheet);
            WriteArcFlashLabelSection(rpt, excelSheet);
            WriteMiscSection(rpt, excelSheet);
            WriteReportMetadata(rpt, excelSheet);
            var ms2 = new MemoryStream();
            workbook.SaveAs(ms2);
            ms2.Position = 0;
            return ms2;



        }

        private void WriteMiscSection(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {
               var rowStart = 23 + report.Data.TimeAndExpense.Count() + report.Data.CompanyVehicle.Count() + report.Data.SubContractor.Count() + report.Data.ArcFlashLabel.Count();
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.Misc.Count());

            foreach (var sectionRow in report.Data.Misc)
            {

                excelSheet.Range(rowStart, 1, rowStart,4).Merge()                    
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AssignValue(sectionRow.Description)
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                
                excelSheet.Cell(rowStart, 5)
                    .AssignValue(sectionRow.Cost)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .SetFontStyle(f => f.Bold = false)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);
               
                rowStart++;
            }

             excelSheet.Range(rowStart, 1, rowStart,4).Merge()         
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddRightBorder(XLBorderStyleValues.Thin)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AssignValue("TOTAL")
               .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
               .SetFontStyle(x => x.Bold = true);            

            var labelTotal = excelSheet.Cell(rowStart, 5)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);
            labelTotal.SetFontStyle(x => x.Bold = true);
            labelTotal.FormulaA1 = $"SUM({columns[5]}{originalRowStart}:{columns[5]}{rowStart - 1})";
            labelTotal.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);
        }

        private void WriteArcFlashLabelSection(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {
             var rowStart = 19 + report.Data.TimeAndExpense.Count() + report.Data.CompanyVehicle.Count() + report.Data.SubContractor.Count();
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.ArcFlashLabel.Count());

            foreach (var sectionRow in report.Data.ArcFlashLabel)
            {

                excelSheet.Cell(rowStart, 1)
                    .MergeWith(FluentCellStyle.CellToThe.right)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .SetDataType(XLDataType.DateTime)
                    .AssignValue(DateTimeWithZone.ConvertToEST(sectionRow.DateOfInvoice.UtcDateTime))
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                
                excelSheet.Cell(rowStart, 3)
                    .AssignValue(sectionRow.Quantity)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .SetFontStyle(f => f.Bold = false)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                excelSheet.Cell(rowStart, 4)
                    .AssignValue(sectionRow.LabelCost, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 5)
                    .AssignValue(sectionRow.PostageCost, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 6)
                    .AssignValue(sectionRow.TotalCost, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);
               
                rowStart++;
            }

            excelSheet.Cell(rowStart, 1)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
            

            excelSheet.Cell(rowStart, 2)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
               
            excelSheet.Cell(rowStart, 3).AssignValue("TOTAL")                
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)                                
                .SetFontStyle(x => x.Bold = true);

            var labelTotal = excelSheet.Cell(rowStart, 4)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);
            labelTotal.SetFontStyle(x => x.Bold = true);
            labelTotal.FormulaA1 = $"SUM({columns[4]}{originalRowStart}:{columns[4]}{rowStart - 1})";
            labelTotal.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);

            var postageTotal = excelSheet.Cell(rowStart, 5)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);
            postageTotal.SetFontStyle(x => x.Bold = true);
            postageTotal.FormulaA1 = $"SUM({columns[5]}{originalRowStart}:{columns[5]}{rowStart - 1})";
            postageTotal.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);

            var totalCell = excelSheet.Cell(rowStart, 6)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);
            totalCell.SetFontStyle(x => x.Bold = true);
            totalCell.FormulaA1 = $"SUM({columns[6]}{originalRowStart}:{columns[6]}{rowStart - 1})";
            totalCell.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);
        }

        private void WriteSubContractorSection(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {
            
            var rowStart = 15 + report.Data.TimeAndExpense.Count() + report.Data.CompanyVehicle.Count();
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.SubContractor.Count());

            foreach (var sectionRow in report.Data.SubContractor)
            {

                excelSheet.Cell(rowStart, 1)
                    .MergeWith(FluentCellStyle.CellToThe.right)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)                    
                    .AssignValue(sectionRow.Company)
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Dotted)                    
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                excelSheet.Cell(rowStart, 3)
                    .AssignValue(sectionRow.PONumber)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .SetFontStyle(f => f.Bold = false)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                excelSheet.Cell(rowStart, 4)
                    .AssignValue(sectionRow.ContractAmount, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);
               
                rowStart++;
            }

            excelSheet.Cell(rowStart, 1)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
            

            excelSheet.Cell(rowStart, 2)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);

            excelSheet.Cell(rowStart, 3).AssignValue("TOTAL")
                
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)                                
                .SetFontStyle(x => x.Bold = true);

            var totalCell = excelSheet.Cell(rowStart, 4)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);

            totalCell.SetFontStyle(x => x.Bold = true);
            totalCell.FormulaA1 = $"SUM({columns[4]}{originalRowStart}:{columns[4]}{rowStart - 1})";

            totalCell.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);
        }

        private static void SetHeaderValues(ReportDTO<DetailedExpenseForJobReportDTO> rpt, IXLWorksheet excelSheet)
        {
            excelSheet.Cell(1, 2).Value = (rpt.Data.JobCode);
            excelSheet.Cell(1, 4).Value = (rpt.Data.JobName);

            excelSheet.Cell(2, 2).Value = (rpt.Data.SiteName);
            excelSheet.Cell(2, 5).Value = (rpt.Data.ClientName);

            excelSheet.Cell(3, 5).Value = $"{rpt.Data.PeriodStart.ToShortDateString()} thru {rpt.Data.PeriodEnd.ToShortDateString()}";

        }

        private static void WriteReportMetadata(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {
            var rowStart = 25 + report.Data.TimeAndExpense.Count() + report.Data.CompanyVehicle.Count() + report.Data.SubContractor.Count() + report.Data.ArcFlashLabel.Count() + report.Data.Misc.Count();


            var row = rowStart;
            
            foreach (var item in report.RunSettings)
            {
                 excelSheet.Range(rowStart, 1, rowStart,4).Merge()                    
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                    .AssignValue($"{item.Key}: {item.Value}")
                    .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                    .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                    .AddTopBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                    .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);

                rowStart++;
               
            }
        }

        private static void WriteTimeAndExpensesSection(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {

            

            var rowStart = 7;
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.TimeAndExpense.Count());

            excelSheet.Range($"{columns[1]}{rowStart - 1}:{columns[2]}{rowStart - 1}")
                .AddBottomBorder(XLBorderStyleValues.Medium);

            foreach (var sectionRow in report.Data.TimeAndExpense)
            {

                excelSheet.Cell(rowStart, 1)
                    .MergeWith(FluentCellStyle.CellToThe.right)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .SetDataType(XLDataType.DateTime)
                    .AssignValue(DateTimeWithZone.ConvertToEST(sectionRow.Date.UtcDateTime))
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == 7 ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                excelSheet.Cell(rowStart, 3)
                    .AssignValue(sectionRow.EmployeeFirstLast)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == 7 ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)                    
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                excelSheet.Cell(rowStart, 4)
                    .AssignValue(sectionRow.Cost, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .AddTopBorder(rowStart == 7 ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 5)
                    .AddLeftBorder(XLBorderStyleValues.Thin);

                rowStart++;
            }

            excelSheet.Cell(rowStart, 1)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
            

            excelSheet.Cell(rowStart, 2)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);

            excelSheet.Cell(rowStart, 3).AssignValue("TOTAL")
                
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)                                
                .SetFontStyle(x => x.Bold = true);

            var totalCell = excelSheet.Cell(rowStart, 4)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);

            totalCell.SetFontStyle(x => x.Bold = true);
            totalCell.FormulaA1 = $"SUM({columns[4]}{originalRowStart}:{columns[4]}{rowStart - 1})";

            totalCell.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);

        }

        private static void WriteCompanyVehicleSection(ReportDTO<DetailedExpenseForJobReportDTO> report, IXLWorksheet excelSheet)
        {
            

            var rowStart = 11 + report.Data.TimeAndExpense.Count();
            var originalRowStart = rowStart;
            excelSheet.Row(rowStart).InsertRowsBelow(report.Data.CompanyVehicle.Count());

            excelSheet.Range($"{columns[1]}{rowStart - 1}:{columns[2]}{rowStart - 1}")
                .AddBottomBorder(XLBorderStyleValues.Medium);
            
            foreach (var sectionRow in report.Data.CompanyVehicle)
            {
                
                excelSheet.Cell(rowStart, 1)
                    .MergeWith(FluentCellStyle.CellToThe.right)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .SetDataType(XLDataType.DateTime)
                    .AssignValue(DateTimeWithZone.ConvertToEST(sectionRow.Date.UtcDateTime))
                    .AddLeftBorder(XLBorderStyleValues.Thin)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                   excelSheet.Cell(rowStart, 3)
                    .AssignValue(sectionRow.Vehicle)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                  excelSheet.Cell(rowStart, 4)
                    .AssignValue(sectionRow.EmployeeFirstLast)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Left)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 5)
                    .AssignValue(sectionRow.NumberOfDays)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin);

                excelSheet.Cell(rowStart, 6)
                    .AssignValue(sectionRow.TotalMiles)
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Dotted)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .AddBottomBorder(XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false);

                  excelSheet.Cell(rowStart, 7)
                    .AssignValue(sectionRow.Cost, dataFormatOverride: (XLDataType.Number, "$#,###,##0.00"))
                    .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                    .AddLeftBorder(XLBorderStyleValues.Dotted)
                    .AddRightBorder(XLBorderStyleValues.Thin)
                    .AddTopBorder(rowStart == originalRowStart ? XLBorderStyleValues.Medium : XLBorderStyleValues.Thin)
                    .SetFontStyle(f => f.Bold = false)
                    .AddBottomBorder(XLBorderStyleValues.Thin);


                rowStart++;
            }

              excelSheet.Cell(rowStart, 1)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
            

            excelSheet.Cell(rowStart, 2)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);


             excelSheet.Cell(rowStart, 3)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
             excelSheet.Cell(rowStart, 4)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);
             excelSheet.Cell(rowStart, 5)
               .AddTopBorder(XLBorderStyleValues.Thin)
               .AddRightBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)
               .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray);

            excelSheet.Cell(rowStart, 6).AssignValue("TOTAL")
                
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Right)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddBottomBorder(XLBorderStyleValues.Thin, XLColor.Gray)
                .AddLeftBorder(XLBorderStyleValues.Thin, XLColor.Gray)                                
                .SetFontStyle(x => x.Bold = true);

            var totalCell = excelSheet.Cell(rowStart, 7)
                .AddRightBorder(XLBorderStyleValues.Thin)
                .SetAlignHorizontal(XLAlignmentHorizontalValues.Center)
                .AddBottomBorder(XLBorderStyleValues.Thin)
                .AddLeftBorder(XLBorderStyleValues.Thin)
                .AddTopBorder(XLBorderStyleValues.Thin)
                .AddRightBorder(XLBorderStyleValues.Thin);

            totalCell.SetFontStyle(x => x.Bold = true);
            totalCell.FormulaA1 = $"SUM({columns[7]}{originalRowStart}:{columns[7]}{rowStart - 1})";

            totalCell.CellRight().AddLeftBorder(XLBorderStyleValues.Thin);

        }

      

    }
}
