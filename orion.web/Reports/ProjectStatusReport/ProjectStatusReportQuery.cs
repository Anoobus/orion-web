using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IJobSummaryQuery
    {
        Task<ReportDTO<ProjectStatusReportDTO>> RunAsync(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName, int jobId);
    }
    public class ProjectStatusReportQuery : IJobSummaryQuery
    {
        private readonly IConfiguration configuration;

        public ProjectStatusReportQuery(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<ReportDTO<ProjectStatusReportDTO>> RunAsync(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName, int jobId)
        {
            using(var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @"

Select " +

 $"	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as  {nameof(ProjectStatusReportDTO.PeriodStart)}, " 
+ $"	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as {nameof(ProjectStatusReportDTO.PeriodEnd)},    "
+ $"	e.First + ', ' + e.Last  as  {nameof(JobEmployees.EmployeeName)}, "
+ $"	c.clientcode + '-' + j.JobCode  as  {nameof(ProjectStatusReportDTO.JobCode)}, "
+ $"	j.JobName as  {nameof(ProjectStatusReportDTO.JobName)},"
+ $"	c.ClientName as  {nameof(ProjectStatusReportDTO.ClientName)}, " 
+ $"    s.SiteName as  {nameof(ProjectStatusReportDTO.SiteName)}, "
+ $"	jt.ShortName as {nameof(JobEmployees.TaskName)}, "
+ $"	tc.Name as {nameof(JobEmployees.TaskCategory)}, "
+ $"	isnull(sum(te.hours),0) as {nameof(JobEmployees.Regular)}, "
+ $"    isnull(sum(te.overtimehours),0) as {nameof(JobEmployees.Overtime)}, " 
+ $"	isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) as  {nameof(JobEmployees.Combined)}" 
+

@"
from 
    dbo.Jobs j
    inner join dbo.Clients c
        on c.ClientId = j.ClientId
	left outer join [dbo].TimeEntries te
		on j.JobId = te.JobId
    left outer join [dbo].Employees e
		on e.EmployeeId = te.EmployeeId
    inner join dbo.[Sites] s
	    on j.SiteId = s.SiteID
	left outer join dbo.JobTasks jt
		on jt.JobTaskId = te.TaskId
    inner join dbo.TaskCategories tc
	    on tc.TaskCategoryId = jt.TaskCategoryId
where 
	(@JobId is null Or te.JobId = @JobId) and
	te.Date >= @WeekStart and te.Date <= @WeekEnd
   group by tc.Name,s.SiteName, c.clientcode + '-' + j.JobCode, e.First + ', ' + e.Last , c.clientcode + j.JobCode  , j.JobName, c.ClientName , jt.ShortName, j.JobId
	";


                cmd.Parameters.Add(new SqlParameter("JobId", jobId));

                cmd.Parameters.Add(new SqlParameter("WeekStart", start));
                cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

                var rdr = cmd.ExecuteReader();

                var map = GetColumnMap(rdr.GetColumnSchema());
                var rpt = new ProjectStatusReportDTO()
                {
                    PeriodEnd = end,
                    PeriodStart = start

                };
                var firstRowSettingsRetrieved = false;
                var employeeRows = new List<JobEmployees>();
                while (await rdr.ReadAsync())
                {

                    if(!firstRowSettingsRetrieved)
                    {
                        rpt.JobCode = rdr.GetSqlString(map[nameof(ProjectStatusReportDTO.JobCode)]).Value;
                        rpt.JobName = rdr.GetSqlString(map[nameof(ProjectStatusReportDTO.JobName)]).Value;
                        rpt.SiteName = rdr.GetSqlString(map[nameof(ProjectStatusReportDTO.SiteName)]).Value;
                        rpt.ClientName = rdr.GetSqlString(map[nameof(ProjectStatusReportDTO.ClientName)]).Value;
                        firstRowSettingsRetrieved = true;
                    }
                    employeeRows.Add(new JobEmployees()
                    {
                        Combined = rdr.GetDecimal(map[nameof(JobEmployees.Combined)]),
                        Regular = rdr.GetDecimal(map[nameof(JobEmployees.Regular)]),
                        Overtime = rdr.GetDecimal(map[nameof(JobEmployees.Overtime)]),
                        EmployeeName = rdr.GetSqlString(map[nameof(JobEmployees.EmployeeName)]).Value,
                         TaskCategory = rdr.GetSqlString(map[nameof(JobEmployees.TaskCategory)]).Value,
                        TaskName = rdr.GetSqlString(map[nameof(JobEmployees.TaskName)]).Value,
                    });
                }
                rpt.Employees = employeeRows;
                return new ReportDTO<ProjectStatusReportDTO>()
                {
                    Data = rpt,
                    ReportName = "Pay Period Report",
                    RunSettings =
                new Dictionary<string, string>()
                {                   
                    { "Generated", $"{DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}"},
                    { "Company", $"Orion Engineering Co., Inc." },
                }
                };


            }
        }
        private static readonly Dictionary<string, int> ColumnMap = new Dictionary<string, int>();
        private static object reportColumnMapLock = new object();
        private Dictionary<string, int> GetColumnMap(ReadOnlyCollection<DbColumn> cols)
        {
            lock (reportColumnMapLock)
            {
                if (!ColumnMap.Any())
                {
                    var rptPropNames = typeof(ProjectStatusReportDTO).GetProperties();
                    foreach (var prop in rptPropNames)
                    {
                        var match = cols.SingleOrDefault(x => x.ColumnName == prop.Name);
                        if(match != null)
                        {                        
                            ColumnMap.Add(prop.Name, match.ColumnOrdinal.Value);
                        }
                    }
                    var empPropNames = typeof(JobEmployees).GetProperties();
                    foreach (var prop in empPropNames)
                    {
                        var match = cols.SingleOrDefault(x => x.ColumnName == prop.Name);
                        if (match != null)
                        {
                            ColumnMap.Add(prop.Name, match.ColumnOrdinal.Value);
                        }
                    }
                }

            }
            return ColumnMap;
        }

    }
}
