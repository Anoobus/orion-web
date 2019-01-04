using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IJobSummaryQuery
    {
        ReportDTO<DataTable> Run(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName);
    }
    public class JobSummaryQuery : IJobSummaryQuery
    {
        private readonly IConfiguration configuration;

        public JobSummaryQuery(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public ReportDTO<DataTable> Run(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName)
        {
            using(var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"

Select 
	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as PeriodStarting, 
	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as PeriodEnding,    
	isnull(sum(te.hours),0) as regular, isnull(sum(te.overtimehours),0) as overtime, 
	isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) as combined,
	c.clientcode + '-' + j.JobCode  as JobCode, 
	j.JobName,
	c.ClientName  
from 
    dbo.Jobs j
    inner join dbo.Clients c
    on c.ClientId = j.ClientId
	left outer join [dbo].TimeEntries te
		on j.JobId = te.JobId
    left outer join [dbo].Employees e
		on e.EmployeeId = te.EmployeeId
	left outer join dbo.JobTasks jt
		on jt.JobTaskId = te.TaskId
where 
	Isnull(te.Date,@WeekStart) >= @WeekStart
    and isnull( te.Date,@WeekEnd) <= @WeekEnd
    group by c.clientcode + '-' + j.JobCode, c.clientcode + j.JobCode  , j.JobName, c.ClientName
	having 
	case 
		when isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) != 0 then 1
		when isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) = 0 then IsNull(@IncludeEmpty, 0)
		else 0
	end = 1";


                cmd.Parameters.Add(new SqlParameter("IncludeEmpty", showEmptyJobs));

                cmd.Parameters.Add(new SqlParameter("WeekStart", start));
                cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

                var rdr = cmd.ExecuteReader();

                return MapToReport(rdr, reportDisplayName, new Dictionary<string, string>()
                {
                    { "Include Jobs with no time?", showEmptyJobs.ToString()},
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
