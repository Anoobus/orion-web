using Microsoft.Extensions.Configuration;
using orion.web.Common;
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
    public interface IJobSummaryReportQuery
    {
        Task<ReportDTO<JobSummaryReportDTO>> RunAsync(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName, int jobId);
    }
    public class JobSummaryReportQuery : IJobSummaryReportQuery
    {
        private readonly IConfiguration configuration;

        public JobSummaryReportQuery(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<ReportDTO<JobSummaryReportDTO>> RunAsync(DateTime start, DateTime end, bool showEmptyJobs, string reportDisplayName, int jobId)
        {
            using(var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @"

Select " +

 $"	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as  {nameof(JobSummaryReportDTO.PeriodStart)}, "
+ $"	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as {nameof(JobSummaryReportDTO.PeriodEnd)},    "
+ $"	e.First + ', ' + e.Last  as  {nameof(JobEmployees.EmployeeName)}, "
+ $"	c.clientcode + '-' + j.JobCode  as  {nameof(JobSummaryReportDTO.JobCode)}, "
+ $"	j.JobName as  {nameof(JobSummaryReportDTO.JobName)},"
+ $"	c.ClientName as  {nameof(JobSummaryReportDTO.ClientName)}, "
+ $"    s.SiteName as  {nameof(JobSummaryReportDTO.SiteName)}, "
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
                var rpt = new JobSummaryReportDTO()
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
                        rpt.JobCode = rdr.GetSqlString(map[nameof(JobSummaryReportDTO.JobCode)]).Value;
                        rpt.JobName = rdr.GetSqlString(map[nameof(JobSummaryReportDTO.JobName)]).Value;
                        rpt.SiteName = rdr.GetSqlString(map[nameof(JobSummaryReportDTO.SiteName)]).Value;
                        rpt.ClientName = rdr.GetSqlString(map[nameof(JobSummaryReportDTO.ClientName)]).Value;
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
                return new ReportDTO<JobSummaryReportDTO>()
                {
                    Data = rpt,
                    ReportName = "Pay Period Report",
                    RunSettings =
                new Dictionary<string, string>()
                {
                    { "Generated", $"{DateTimeWithZone.EasternStandardTime.ToShortDateString()} at {DateTimeWithZone.EasternStandardTime.ToShortTimeString()}"},
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
                    var rptPropNames = typeof(JobSummaryReportDTO).GetProperties();
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
