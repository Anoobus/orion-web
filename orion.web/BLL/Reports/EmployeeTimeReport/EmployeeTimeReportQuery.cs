using Microsoft.Extensions.Configuration;
using orion.web.Clients;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.Reports.EmployeeTimeReport;
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

namespace orion.web.Reports.EmployeeTimeReport
{
    public interface IEmployeeTimeReportQuery
    {
        Task<ReportDTO<EmployeeTimeReportDTO>> RunAsync(EmployeeTimeReportCriteria criteria);
    }
    public class EmployeeTimeReportQuery : IEmployeeTimeReportQuery, IAutoRegisterAsSingleton
    {
        private readonly IConfiguration configuration;
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeTimeReportQuery(IConfiguration configuration,
            ISessionAdapter sessionAdapter, IEmployeeRepository employeeRepository
         )
        {
            this.configuration = configuration;
            _sessionAdapter = sessionAdapter;
            _employeeRepository = employeeRepository;
        }
        public async Task<ReportDTO<EmployeeTimeReportDTO>> RunAsync(EmployeeTimeReportCriteria criteria)
        {
            DateTime start = criteria.PeriodSettings.Start;
            DateTime end = criteria.PeriodSettings.End;
            
                                   

            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @"

SELECT
      
      ISNULL(j.JobCode,'') + ' - ' + ISNULL(j.JobName,'') as Job
      ,ISNULL(jt.LegacyCode,'') + ' - ' + ISNULL(jt.Name,'') as Task  
      ,SUM(ISNULL([Hours],0)) as RegularHours
      ,SUM(ISNULL([OvertimeHours],0)) as OvertimeHours
      
FROM [orion.web].[dbo].[TimeEntries] te
inner join [orion.web].[dbo].[Jobs] j
  on te.JobId = j.JobId
inner join [orion.web].[dbo].[JobTasks] jt
  on jt.JobTaskId = te.TaskId
where te.EmployeeId = @employeeId
  and [Date] BETWEEN @startInclusive and @endInclusive
group by        
    ISNULL(j.JobCode,'') + ' - ' + ISNULL(j.JobName,'')
    ,ISNULL(jt.LegacyCode,'') + ' - ' + ISNULL(jt.Name,'') 
order by 1,2";




                cmd.Parameters.Add(new SqlParameter("employeeId", criteria.SelectedEmployeeId));
                cmd.Parameters.Add(new SqlParameter("startInclusive", start));
                cmd.Parameters.Add(new SqlParameter("endInclusive", end));
                

                var rdr = cmd.ExecuteReader();
                var emp = await _employeeRepository.GetSingleEmployeeAsync(criteria.SelectedEmployeeId);

                var rez = new EmployeeTimeReportDTO()
                {
                    EmployeeName = $"{emp.Last}, {emp.First}",
                    PeriodEnd = end,
                    PeriodStart = start                     
                };
                var rows = new List<EmployeeTimeEntry>();
                while (await rdr.ReadAsync())
                {
                    if (rdr.HasRows)
                    {
                        rows.Add(new EmployeeTimeEntry()
                        {
                             JobCode = rdr.GetString(0),
                             TaskCode = rdr.GetString(1),
                             Regular = rdr.GetDecimal(2),
                             Overtime = rdr.GetDecimal(3)                            
                        });
                    }
                }
                rez.Entries = rows;
                    
                return new ReportDTO<EmployeeTimeReportDTO>()
                {
                    Data = rez,
                    ReportName = EmployeeTimeReportCriteria.EMPLOYEE_TIME_REPORT_NAME,
                    RunSettings =
                new Dictionary<string, string>()
                {
                    { "Generated", $"{DateTimeWithZone.EasternStandardTime.ToShortDateString()} at {DateTimeWithZone.EasternStandardTime.ToShortTimeString()}"},
                    { "Company", $"Orion Engineering Co., Inc." }
                }};


            }
        }      
    }
}
