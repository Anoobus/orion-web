using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace orion.web.Reports
{
    public class ExcelExport
    {
        public MemoryStream AsXls(ReportDTO rpt)
        {
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = Guid.NewGuid();
            try
            {
                using(var fs = new FileStream($"{fName}.xlsx", FileMode.Create))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet(rpt.ReportName);
                    IRow headerRow = excelSheet.CreateRow(0);

                    foreach(var col in rpt.Data.Columns.OfType<DataColumn>().Select((x, index) => new { val = x, index }))
                    {
                        headerRow.CreateCell(col.index).SetCellValue(col.val.ColumnName);
                    }

                    foreach(var dataRow in rpt.Data.Rows.OfType<DataRow>().Select((x, index) => new { val = x, index }))
                    {
                        IRow row = excelSheet.CreateRow(dataRow.index + 1);
                        for(int i = 0; i < headerRow.Cells.Count; i++)
                        {
                            row.CreateCell(i).SetCellValue(dataRow.val[i].ToString());
                        }
                    }
                    workbook.Write(fs);
                }
                using(var fs = new FileStream($"{fName}.xlsx", FileMode.Open))

                {
                    fs.CopyTo(ms2);
                }
                ms2.Position = 0;
                return ms2;
            }
            finally
            {
                if(File.Exists($"{fName}.xlsx"))
                {
                    try
                    {
                        File.Delete($"{fName}.xlsx");
                    }
                    catch(Exception)
                    {
                        // eat it
                    }
                }
            }

        }

    }
}
