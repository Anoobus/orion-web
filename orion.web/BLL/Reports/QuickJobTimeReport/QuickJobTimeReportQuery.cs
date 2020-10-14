using Microsoft.Extensions.Configuration;
using orion.web.Clients;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.QuickJobTimeReport;
using orion.web.Util.IoC;
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
    public interface IQuickJobTimeReportQuery
    {
        Task<ReportDTO<QuickJobTimeReportDTO>> RunAsync(QuickJobTimeReportCriteria criteria);
    }
    public class QuickJobTimeReportQuery : IQuickJobTimeReportQuery, IAutoRegisterAsSingleton
    {
        private readonly IConfiguration configuration;
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IJobsRepository _jobsRepository;
        private readonly ISitesRepository _sitesRepository;
        private readonly IClientsRepository _clientsRepository;

        public QuickJobTimeReportQuery(IConfiguration configuration, ISessionAdapter sessionAdapter, IJobsRepository jobsRepository, ISitesRepository sitesRepository, IClientsRepository clientsRepository)
        {
            this.configuration = configuration;
            _sessionAdapter = sessionAdapter;
            _jobsRepository = jobsRepository;
            _sitesRepository = sitesRepository;
            _clientsRepository = clientsRepository;
        }
        public async Task<ReportDTO<QuickJobTimeReportDTO>> RunAsync(QuickJobTimeReportCriteria criteria)
        {
            DateTime start = criteria.PeriodSettings.Start;
            DateTime end = criteria.PeriodSettings.End;
            int jobId = int.Parse(criteria.SelectedJobId);

            int? limitToEmployeeId = await _sessionAdapter.EmployeeIdAsync();
            if(criteria.ShowAllEmployeesForJob)
                limitToEmployeeId = null;

            using(var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @"

Select " +

 $"	min(Convert(varchar(10),Isnull(te.Date,@WeekStart), 101)) as  {nameof(QuickJobTimeReportDTO.PeriodStart)}, "
+ $"	max(Convert(varchar(10),isnull( te.Date,@WeekEnd),101)) as {nameof(QuickJobTimeReportDTO.PeriodEnd)},    "
+ $"	COALESCE(e.Last,'') + ', ' + COALESCE(e.First,'')  as  {nameof(QuickJobEmployees.EmployeeName)}, "
+ $"	j.JobCode  as  {nameof(QuickJobTimeReportDTO.JobCode)}, "
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
	te.Date >= @WeekStart and te.Date <= @WeekEnd and
    ISNULL(e.[UserName],'') != 'admin@company.com' "
+ (limitToEmployeeId.HasValue ? $" and te.EmployeeId = { limitToEmployeeId.Value}" : string.Empty)
+ @"  group by tc.Name,s.SiteName, 
j.JobCode, 
COALESCE(e.Last,'') + ', ' + COALESCE(e.First,'') , 
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

                var employeeRows = new List<QuickJobEmployees>();

                var job = (await _jobsRepository.GetAsync()).SingleOrDefault(x => x.JobId == jobId);
                rpt.JobCode = job.JobCode;
                rpt.JobName = job.JobName;
                rpt.SiteName = (await _sitesRepository.GetAll()).SingleOrDefault(x => x.SiteID == job.SiteId)?.SiteName;
                rpt.ClientName = (await _clientsRepository.GetClient(job.ClientId))?.ClientName;

                while (await rdr.ReadAsync())
                {
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
                    { "Showing Time From", criteria.ShowAllEmployeesForJob ? "All Employees" : "Self"  },
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
