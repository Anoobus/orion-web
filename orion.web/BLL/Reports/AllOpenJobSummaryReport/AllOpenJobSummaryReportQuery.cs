using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using orion.web.Common;
using orion.web.Util.IoC;

namespace orion.web.Reports
{
    public interface IAllOpenJobSummaryReportQuery
    {
        Task<ReportDTO<AllOpenJobSummaryReportDTO>> RunAsync(bool showEmptyJobs, string reportDisplayName);
    }
    public class AllOpenJobSummaryReportQuery : IAllOpenJobSummaryReportQuery , IAutoRegisterAsSingleton
    {
        private readonly IConfiguration _configuration;

        public AllOpenJobSummaryReportQuery(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public class TempCoreData
        {
            public int JobId { get; set; }
            public int EmployeeId { get; set; }
            public decimal TotalHours { get; set; }
        }
        public async Task<ReportDTO<AllOpenJobSummaryReportDTO>> RunAsync(bool showEmptyJobs, string reportDisplayName)
        {
            
          
            using(var conn = new SqlConnection(_configuration.GetConnectionString("SiteConnection")))
            using(var cmd = conn.CreateCommand())
            {
                conn.Open();


                cmd.CommandText = @";with CoreData as (
                                    SELECT
                                        te.JobId,
                                        te.EmployeeId,
                                        SUM(ISNULL(Hours, 0)) + SUM(ISNULL(OvertimeHours, 0)) as TotalHours
                                    FROM     dbo.TimeEntries te
                                    inner join dbo.Jobs j
                                        on j.JobId = te.JobId
                                    where j.JobStatusId = 1
                                    group by te.EmployeeId,  te.JobId)

                                    SELECT * FROM CoreData

                                    Select distinct e.EmployeeId
                                    ,e.First + ', ' + e.Last  as  EmployeeName
                                    FROM dbo.Employees e
                                    where e.EmployeeId != 1;

                                    select j.JobId,
                                    j.JobCode
                                    from dbo.Jobs j


                                    select j.JobId
                                    ,ISNULL(tna.amount,0) 
                                     + ISNULL(vea.amount,0) 
                                     + ISNULL(cea.amount,0) 
                                     + ISNULL(afla.amount,0) 
                                     + ISNULL(mea.amount,0) as TotalAmount
                                    from dbo.Jobs j
                                    left outer join (
                                        Select 
                                            te.JobId
                                            ,SUM(ISNULL(te.Amount,0)) as amount
                                        from dbo.TimeAndExpenceExpenditures te
                                        group by te.JobId) as tna
                                    on tna.JobId = j.Jobid
                                    left outer join (
                                        select 
                                            ve.JobId
                                            ,SUM(ISNULL((ve.TotalNumberOfDaysUsed * 125)
                                                + CASE WHEN ve.TotalMiles > 250 
                                                    THEN (ve.TotalMiles * .50) - (250 * .50)
                                                    ELSE 0
                                                    END,0)) as amount
                                        from dbo.CompanyVehicleExpenditures ve
                                        group by ve.JobId) as vea
                                    on vea.JobId = j.JobId
                                    left outer join (
                                        select 
                                            ce.JobId
                                            ,SUM(ISNULL(ce.TotalPOContractAmount,0)) as amount
                                        from dbo.ContractorExpenditures ce
                                        group by ce.JobId
                                    ) as cea
                                    on cea.JobId = j.JobId

                                    left outer join (
                                        select 
                                            afl.JobId,
                                            SUM(ISNULL(afl.TotalLabelsCost,0) + ISNULL(afl.TotalPostageCost,0)) as amount
                                        from dbo.ArcFlashlabelExpenditures afl
                                        group by afl.JobId
                                    ) as afla
                                    on afla.JobId = j.JobId
                                    left outer join (
                                        select 
                                            e.JobId
                                            ,SUM(ISNULL(e.Amount,0)) as amount
                                        from dbo.MiscExpenditures e
                                        group by e.JobId
                                    ) as mea
                                    on mea.JobId = j.JobId

                                    where ISNULL(tna.amount,0) 
                                     + ISNULL(vea.amount,0) 
                                     + ISNULL(cea.amount,0) 
                                     + ISNULL(afla.amount,0) 
                                     + ISNULL(mea.amount,0) > 0";

                

                var rdr = cmd.ExecuteReader();
                var core = new List<TempCoreData>();
                while(await rdr.ReadAsync())
                {
                    core.Add(new TempCoreData()
                    {
                        EmployeeId = rdr.GetInt32(1),
                        JobId = rdr.GetInt32(0),
                        TotalHours = rdr.GetDecimal(2)
                    });
                }
                await rdr.NextResultAsync();
                var employees = new Dictionary<int, string>();
                while(await rdr.ReadAsync())
                {
                    employees.Add(rdr.GetInt32(0), rdr.GetString(1));
                }

                await rdr.NextResultAsync();
                var jobs = new Dictionary<int, string>();
                while(await rdr.ReadAsync())
                {
                    jobs.Add(rdr.GetInt32(0), rdr.GetString(1));
                }


                var rpt = new AllOpenJobSummaryReportDTO()
                {
                    Rows = new Dictionary<string, JobSummaryEmployeeCells>(),
                    EmployeeIdToNameMap = new Dictionary<int, string>(),
                    ExpenseAmountForJob = new Dictionary<string, decimal>()
                };
                rpt.EmployeeIdToNameMap = employees;

                await rdr.NextResultAsync();
                while(await rdr.ReadAsync())
                {
                    var jobName = jobs[rdr.GetInt32(0)];
                    rpt.ExpenseAmountForJob.Add(jobName, rdr.GetDecimal(1));
                }
            
                var hmm = core.GroupBy(x => x.JobId);
                foreach (var job in hmm)
                {
                    var jobCode = jobs[job.Key];
                    rpt.Rows.Add(jobCode, new JobSummaryEmployeeCells(job.ToDictionary(j => j.EmployeeId, j => j.TotalHours)));
                }
               
                return new ReportDTO<AllOpenJobSummaryReportDTO>()
                {
                    Data = rpt,
                    ReportName = "All Open Job Summary Report",
                    RunSettings =
                new Dictionary<string, string>()
                {
                    { "Generated", $"{DateTimeWithZone.EasternStandardTime.ToShortDateString()} at {DateTimeWithZone.EasternStandardTime.ToShortTimeString()}"},
                    { "Company", $"Orion Engineering Co., Inc." },
                }
                };
            }
            
        }
    }
}
