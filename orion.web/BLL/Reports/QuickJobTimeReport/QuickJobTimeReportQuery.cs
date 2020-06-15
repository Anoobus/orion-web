using Microsoft.Extensions.Configuration;
using orion.web.Common;
using orion.web.Reports.QuickJobTimeReport;
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
    public interface IQuickJobTimeReportQuery : IRegisterByConvention
    {
        Task<ReportDTO<QuickJobTimeReportDTO>> RunAsync(QuickJobTimeReportCriteria criteria);
    }
    public class QuickJobTimeReportQuery : IQuickJobTimeReportQuery
    {
        private readonly IConfiguration configuration;

        public QuickJobTimeReportQuery(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<ReportDTO<QuickJobTimeReportDTO>> RunAsync(QuickJobTimeReportCriteria criteria)
        {
            DateTime start = criteria.PeriodSettings.Start;
            DateTime end = criteria.PeriodSettings.End;
            int jobId = int.Parse(criteria.SelectedJobId);

            using(var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @"

Select " +

 $"	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as  {nameof(QuickJobTimeReportDTO.PeriodStart)}, " 
+ $"	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as {nameof(QuickJobTimeReportDTO.PeriodEnd)},    "
+ $"	COALESCE(e.Last,'') + ', ' + COALESCE(e.First,'')  as  {nameof(QuickJobEmployees.EmployeeName)}, "
+ $"	c.clientcode + '-' + j.JobCode  as  {nameof(QuickJobTimeReportDTO.JobCode)}, "
+ $"	j.JobName as  {nameof(QuickJobTimeReportDTO.JobName)},"
+ $"	c.ClientName as  {nameof(QuickJobTimeReportDTO.ClientName)}, " 
+ $"    s.SiteName as  {nameof(QuickJobTimeReportDTO.SiteName)}, "
+ $"	jt.LegacyCode + ' - ' + jt.[Name] as {nameof(QuickJobEmployees.TaskName)}, "
+ $"	tc.Name as {nameof(QuickJobEmployees.TaskCategory)}, "
+ $"	isnull(sum(te.hours),0) as {nameof(QuickJobEmployees.Regular)}, "
+ $"    isnull(sum(te.overtimehours),0) as {nameof(QuickJobEmployees.Overtime)}, " 
+ $"	isnull(sum(te.hours),0) + isnull(sum(te.overtimehours),0) as  {nameof(QuickJobEmployees.Combined)}" 
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
   group by tc.Name,s.SiteName, 
c.clientcode + '-' + j.JobCode, 
COALESCE(e.Last,'') + ', ' + COALESCE(e.First,'') , 
c.clientcode + j.JobCode  , 
j.JobName, c.ClientName , 
jt.LegacyCode + ' - ' + jt.[Name], 
j.JobId
	";


                cmd.Parameters.Add(new SqlParameter("JobId", jobId));

                cmd.Parameters.Add(new SqlParameter("WeekStart", start));
                cmd.Parameters.Add(new SqlParameter("WeekEnd", end));

                var rdr = cmd.ExecuteReader();

                var map = GetColumnMap(rdr.GetColumnSchema());
                var rpt = new QuickJobTimeReportDTO()
                {
                    PeriodEnd = end,
                    PeriodStart = start

                };
                var firstRowSettingsRetrieved = false;
                var employeeRows = new List<QuickJobEmployees>();
                while (await rdr.ReadAsync())
                {

                    if(!firstRowSettingsRetrieved)
                    {
                        rpt.JobCode = rdr.GetSqlString(map[nameof(QuickJobTimeReportDTO.JobCode)]).Value;
                        rpt.JobName = rdr.GetSqlString(map[nameof(QuickJobTimeReportDTO.JobName)]).Value;
                        rpt.SiteName = rdr.GetSqlString(map[nameof(QuickJobTimeReportDTO.SiteName)]).Value;
                        rpt.ClientName = rdr.GetSqlString(map[nameof(QuickJobTimeReportDTO.ClientName)]).Value;
                        firstRowSettingsRetrieved = true;
                    }
                    if(rdr.HasRows)
                    {
                        employeeRows.Add(new QuickJobEmployees()
                        {
                            Combined = rdr.GetDecimal(map[nameof(QuickJobEmployees.Combined)]),
                            Regular = rdr.GetDecimal(map[nameof(QuickJobEmployees.Regular)]),
                            Overtime = rdr.GetDecimal(map[nameof(QuickJobEmployees.Overtime)]),
                            EmployeeName = rdr.GetSqlString(map[nameof(QuickJobEmployees.EmployeeName)]).Value,
                            TaskCategory = rdr.GetSqlString(map[nameof(QuickJobEmployees.TaskCategory)]).Value,
                            TaskName = rdr.GetSqlString(map[nameof(QuickJobEmployees.TaskName)]).Value,
                        });
                    }
                    }
                    rpt.Employees = employeeRows;
                return new ReportDTO<QuickJobTimeReportDTO>()
                {
                    Data = rpt,
                    ReportName = QuickJobTimeReport.QuickJobTimeReportCriteria.QUICK_JOB_TIME_REPORT_NAME,
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
                    var rptPropNames = typeof(QuickJobTimeReportDTO).GetProperties();
                    foreach (var prop in rptPropNames)
                    {
                        var match = cols.SingleOrDefault(x => x.ColumnName == prop.Name);
                        if(match != null)
                        {                        
                            ColumnMap.Add(prop.Name, match.ColumnOrdinal.Value);
                        }
                    }
                    var empPropNames = typeof(QuickJobEmployees).GetProperties();
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
