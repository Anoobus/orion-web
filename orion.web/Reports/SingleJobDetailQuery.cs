using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace orion.web.Reports
{
    public interface ISingleJobDetailQuery
    {
        ReportDTO<DataTable> Run(DateTime start, DateTime end, int jobId,  string reportDisplayName);
    }
    public class SingleJobDetailQuery : ISingleJobDetailQuery
    {
        private readonly IConfiguration configuration;
        private readonly OrionDbContext db;

        public SingleJobDetailQuery(IConfiguration configuration, OrionDbContext db)
        {
            this.configuration = configuration;
            this.db = db;
        }
        public ReportDTO<DataTable> Run(DateTime start, DateTime end, int jobId, string reportDisplayName)
        {

            var jobMatch = db.Jobs
               .Include(x => x.Client)
               .First(x => x.JobId == jobId);
            var jobName = $"{jobMatch.Client.ClientCode}-{jobMatch.JobCode} {jobMatch.JobName}";

            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
Select 
    min(Convert(varchar(10), te.Date, 101)) as PeriodStarting, 
    max(Convert(varchar(10), te.date, 101)) as PeriodEnding, 
    e.[Name], 
	c.clientcode + '-' + j.JobCode  as JobCode,
	j.JobName,
	c.ClientName ,
	tc.Name as TaskCategory,
	jt.ShortName as TaskName,									
    sum(te.hours) as regular, 
    sum(te.overtimehours) as overtime, 
	sum(te.hours) + sum(te.overtimehours)  as combined
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
where 
	(@JobId is null Or te.JobId = @JobId)
	and te.Date >= @WeekStart
	and te.Date <= @WeekEnd
group by tc.Name, c.clientcode + '-' + j.JobCode, e.[name], c.clientcode + j.JobCode  , j.JobName, c.ClientName , jt.ShortName  ";


                cmd.Parameters.Add(new SqlParameter("JobId", jobId));
                cmd.Parameters.Add(new SqlParameter("WeekStart", start));
                cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

                var rdr = cmd.ExecuteReader();

                return MapToReport(rdr, "AllJobsSummary_TimPeriodReport", new Dictionary<string, string>()
                {
                    { "Job Name", jobName},
                    { "Period Starting", start.ToShortDateString()},
                    { "Period Ending", end.ToShortDateString()},
                });
            }
        }
        private static ReportDTO<DataTable> MapToReport(SqlDataReader rdr, string reportName, Dictionary<string, string> runSettings)
        {
            var rpt = new ReportDTO<DataTable>();
            var dt = new DataTable();
            dt.Load(rdr);
            rpt.Data = dt;
            rpt.ReportName = reportName;
            rpt.RunSettings = runSettings;

            return rpt;
        }

    }
}
