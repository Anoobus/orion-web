using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using orion.web.DataAccess.EF;

namespace orion.web.Reports
{

    public interface IReportservice
    {
        ReportDTO Get(string reportName, DateTime start, DateTime end, int? jobId);
        MemoryStream AsXls(string reportName, DateTime start, DateTime end, int? jobId);
    }

    public class ReportService : IReportservice
    {

        private readonly OrionDbContext db;

        public ReportService(OrionDbContext db)
        {
            this.db = db;
        }
        public ReportDTO Get(string reportName, DateTime start, DateTime end, int? jobId)
        {
            ReportDTO report;
            if(reportName == "TimePeriodReport")
            {
                report = GenerateTimePeriodreport(start, end, jobId);
            }
            else
            {
                report = GenerateDetailedJobreport(start, end, jobId);
            }
            report.ReportName = reportName;
            return report;
        }

        public MemoryStream AsXls(string reportName, DateTime start, DateTime end, int? jobId)
        {
            var rpt = Get(reportName, start, end, jobId);
            var ms2 = new MemoryStream();
            var copy = new MemoryStream();
            var fName = Guid.NewGuid();
            using(var fs = new FileStream($"{fName}.xlsx", FileMode.Create))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet(rpt.ReportName);
                IRow headerRow = excelSheet.CreateRow(0);
                foreach(var col in rpt.Columns.Select((val, index) => new { index, val }))
                {
                    headerRow.CreateCell(col.index).SetCellValue(col.val);
                }

                foreach(var dataRow in rpt.ReportData.Select((val, index) => new { index, val }))
                {
                    IRow row = excelSheet.CreateRow(dataRow.index + 1);
                    foreach(var cell in dataRow.val.Select((val, index) => new { index, val }))
                    {
                        row.CreateCell(cell.index).SetCellValue(cell.val);
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

    private static ReportDTO GenerateDetailedJobreport(DateTime start, DateTime end, int? jobId)
    {
        using(var conn = new SqlConnection("Server=localhost,1401;Database=orion-data;User Id=orion.web;Password=Orion123!;MultipleActiveResultSets=true"))
        using(var cmd = conn.CreateCommand())
        {
            conn.Open();

            cmd.CommandText = @"
                                    Select 
                                        e.[Name], 
                                        min(Convert(varchar(10), te.Date, 101)) as PeriodStarting, 
                                        max(Convert(varchar(10), te.date, 101)) as PeriodEnding, 
                                        sum(te.hours) as regular, 
                                        sum(te.overtimehours) as overtime, jt.ShortName , c.clientcode + j.JobCode  as JobCode, j.JobName, c.ClientName  
                                    from [dbo].TimeEntries te
                                    inner join [dbo].Employees e
                                    on e.EmployeeId = te.EmployeeId
                                    inner join dbo.Jobs j
                                    on j.JobId = te.JobId
                                    inner join dbo.JobTasks jt
                                    on jt.JobTaskId = te.TaskId
                                    inner join dbo.TaskCategories tc
                                    on tc.TaskCategoryId = jt.TaskCategoryId
                                    inner join dbo.Clients c
                                    on c.ClientId = j.ClientId
                                    where (@JobId is null Or te.JobId = @JobId)
                                    and te.Date >= @WeekStart
                                    and te.Date <= @WeekEnd
                                    group by e.[name], c.clientcode + j.JobCode  , j.JobName, c.ClientName , jt.ShortName ";

            if(jobId.HasValue)
            {
                cmd.Parameters.Add(new SqlParameter("JobId", jobId.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("JobId", DBNull.Value));
            }
            cmd.Parameters.Add(new SqlParameter("WeekStart", start));
            cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

            var rdr = cmd.ExecuteReader();

            return MapToReport(rdr);
        }
    }


    private static ReportDTO GenerateTimePeriodreport(DateTime start, DateTime end, int? jobId)
    {
        using(var conn = new SqlConnection("Server=localhost,1401;Database=orion-data;User Id=orion.web;Password=Orion123!;MultipleActiveResultSets=true"))
        using(var cmd = conn.CreateCommand())
        {
            conn.Open();

            cmd.CommandText = @"Select e.[Name], 
                                        min(Convert(varchar(10),te.Date, 101)) as PeriodStarting, 
                                        max(Convert(varchar(10), te.date,101)) as PeriodEnding,    
sum(te.hours) as regular, sum(te.overtimehours) as overtime, c.clientcode + j.JobCode  as JobCode, j.JobName, c.ClientName  
                                    from [dbo].TimeEntries te
                                    inner join [dbo].Employees e
                                    on e.EmployeeId = te.EmployeeId
                                    inner join dbo.Jobs j
                                    on j.JobId = te.JobId
                                    inner join dbo.JobTasks jt
                                    on jt.JobTaskId = te.TaskId
                                    inner join dbo.Clients c
                                    on c.ClientId = j.ClientId
                                    where (@JobId is null Or te.JobId = @JobId)
                                    and te.Date >= @WeekStart
                                    and te.Date <= @WeekEnd
                                    group by e.[name], c.clientcode + j.JobCode  , j.JobName, c.ClientName ";

            if(jobId.HasValue)
            {
                cmd.Parameters.Add(new SqlParameter("JobId", jobId.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("JobId", DBNull.Value));
            }
            cmd.Parameters.Add(new SqlParameter("WeekStart", start));
            cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

            var rdr = cmd.ExecuteReader();

            return MapToReport(rdr);
        }
    }

        private static ReportDTO MapToReport(SqlDataReader rdr)
        {
            var rpt = new ReportDTO()
            {
                ReportData = new List<List<string>>()
            };
            var dt = new DataTable();
            dt.Load(rdr);

            var sum = dt.NewRow();
            sum["regular"] = dt.Rows.OfType<DataRow>().Sum(x => decimal.Parse(x["regular"].ToString()));
            sum["overtime"] = dt.Rows.OfType<DataRow>().Sum(x => decimal.Parse(x["overtime"].ToString()));
            dt.Rows.Add(sum);

            foreach(var row in dt.Rows.OfType<DataRow>())
        {
            var listy = new List<string>();
            foreach(var item in row.ItemArray)
            {
                listy.Add(item.ToString() ?? "");
            }
            rpt.ReportData.Add(listy);
        }
        rpt.Columns = dt.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToArray();
        return rpt;
    }
}
}
