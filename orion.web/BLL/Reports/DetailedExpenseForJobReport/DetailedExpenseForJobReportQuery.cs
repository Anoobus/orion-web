using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using orion.web.BLL.Reports.DetailedExpenseForJobReport;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.Util.IoC;

namespace orion.web.Reports
{
    public interface IDetailedExpenseForJobReportQuery
    {
        Task<ReportDTO<DetailedExpenseForJobReportDTO>> RunAsync(DetailedExpenseForJobReportCriteria settings);
    }
    public class DetailedExpenseForJobReportQuery : IDetailedExpenseForJobReportQuery, IAutoRegisterAsSingleton
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext<DetailedExpenseForJobReportQuery>();

        private static readonly string jobParam = "jobId";
        private static readonly string GetTimeAndExpenseQuery = @"Select 
                                te.ExpenseOnDate,
                                e.First + ', ' + e.Last  as Employee,
                                te.Amount
                                from dbo.TimeAndExpenceExpenditures te
                                inner join dbo.Employees e
                                on te.EmployeeId = e.EmployeeId
                                where te.JobId = @" + jobParam;
        private static TimeAndExposeSectionRow MapToTimeAndExposeSectionRow(SqlDataReader rdr)
        {
            return new TimeAndExposeSectionRow()
            {
                Date = rdr.GetDateTimeOffset(0),
                EmployeeFirstLast = rdr.GetString(1),
                Cost = rdr.GetDecimal(2)
            };
        }
        //$.50 x (600 - (2 x 250))

        private static readonly string GetCompanyVehicleExpenseQuery = @"select 
                                ve.DateVehicleFirstUsed,
                                v.Name,
                                e.First + ', ' + e.Last  as Employee,
                                ve.TotalNumberOfDaysUsed,
                                ve.TotalMiles,
                                (0.5 * (SELECT Max(v)
                                        FROM (VALUES (0), (ve.TotalMiles - (ve.TotalNumberOfDaysUsed * 250))) AS value(v)))
                                + (125 * ve.TotalNumberOfDaysUsed) as Cost
                                from dbo.CompanyVehicleExpenditures ve
                                inner join dbo.CompanyVehicles v
                                on ve.CompanyVehicleId = v.Id
                                inner join dbo.Employees e
                                on e.EmployeeId = ve.EmployeeId
                                where ve.JobId = @" + jobParam;

        private static CompanyVehicleSectionRow MapToCompanyVehicleSectionRow(SqlDataReader rdr)
        {
            return new CompanyVehicleSectionRow()
            {
                Cost = rdr.GetDecimal(5),
                Date = rdr.GetDateTimeOffset(0),
                EmployeeFirstLast = rdr.GetString(2),
                NumberOfDays = rdr.GetInt32(3),
                TotalMiles = rdr.GetInt32(4),
                Vehicle = rdr.GetString(1)
            };
        }

        private static readonly string GetContractorExpenseQuery = @"select 
                                ce.CompanyName,
                                ce.OrionPONumber,
                                ce.TotalPOContractAmount,
                                ce.ExpensedOn
                                from dbo.ContractorExpenditures ce
                                where ce.JobId = @" + jobParam;
        private static SubContractorSectionRow MapToSubContractorSectionRow(SqlDataReader rdr)
        {
            return new SubContractorSectionRow()
            {
                Company = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0),
                PONumber = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
                ContractAmount = rdr.IsDBNull(2) ? 0m : rdr.GetDecimal(2),
                ExpensedOn = rdr.GetDateTimeOffset(3),
            };
        }

        private static readonly string GetArcFlashLabelExpenseQuery = @"select 
                                afl.DateOfInvoice,
                                afl.Quantity,
                                afl.TotalLabelsCost,
                                afl.TotalPostageCost,
                                afl.TotalLabelsCost + afl.TotalPostageCost as TotalCost,
                                e.First + ', ' + e.Last  as Employee
                                from dbo.ArcFlashlabelExpenditures afl
                                left outer join dbo.Employees e
                                 on afl.EmployeeId = e.EmployeeId
                                where afl.JobId = @" + jobParam;
        private static ArcFlashLabeSectionRow MapToArcFlashlabelSection(SqlDataReader rdr)
        {
            return new ArcFlashLabeSectionRow()
            {
                DateOfInvoice = rdr.GetDateTimeOffset(0),
                LabelCost = rdr.GetDecimal(2),
                PostageCost = rdr.GetDecimal(3),
                Quantity = rdr.GetInt32(1),
                TotalCost = rdr.GetDecimal(4),
                EmployeeName = rdr.GetString(5)
            };
        }

         private static readonly string GetMiscExpenseQuery = @"select 
                                e.[Description],
                                e.Amount,
                                e.[ExpensedOn]
                                from dbo.MiscExpenditures e
                                where e.JobId = @" + jobParam;
        private static MiscSectionRow MapToMiscSectionRow(SqlDataReader rdr)
        {
            return new MiscSectionRow()
            {
                Cost = rdr.GetDecimal(1),
                Description = rdr.GetString(0),
                ExpensedOn = rdr.GetDateTimeOffset(2)
            };
        }

        private IConfiguration configuration;
        private readonly IJobsRepository jobsRepository;

        public DetailedExpenseForJobReportQuery(IConfiguration configuration, IJobsRepository jobsRepository)
        {
            this.configuration = configuration;
            this.jobsRepository = jobsRepository;
        }


        private async Task<IEnumerable<T>> LoadSection<T>(string query, Func<SqlDataReader, T> map, int jobId)
        {
            var allResults = new List<T>();
            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqlParameter(jobParam, jobId));
                var rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                {
                    allResults.Add(map(rdr));
                }
            }
            return allResults;
        }
        public async Task<ReportDTO<DetailedExpenseForJobReportDTO>> RunAsync(DetailedExpenseForJobReportCriteria settings)
        {

            try
            {
                var data = new DetailedExpenseForJobReportDTO();
                data.ArcFlashLabel = await LoadSection(GetArcFlashLabelExpenseQuery, MapToArcFlashlabelSection, int.Parse(settings.SelectedJobId));
                data.CompanyVehicle = await LoadSection(GetCompanyVehicleExpenseQuery, MapToCompanyVehicleSectionRow, int.Parse(settings.SelectedJobId));
                data.SubContractor = await LoadSection(GetContractorExpenseQuery, MapToSubContractorSectionRow, int.Parse(settings.SelectedJobId));
                data.TimeAndExpense = await LoadSection(GetTimeAndExpenseQuery, MapToTimeAndExposeSectionRow, int.Parse(settings.SelectedJobId));                
                data.Misc = await LoadSection(GetMiscExpenseQuery, MapToMiscSectionRow, int.Parse(settings.SelectedJobId));
                data.PeriodStart = new DateTime(2021, 1, 1, 10, 0, 0, DateTimeKind.Local);
                data.PeriodEnd = DateTimeWithZone.EasternStandardTime;

                var j = await jobsRepository.GetForJobId(int.Parse(settings.SelectedJobId));
                data.JobCode = j.CoreInfo.JobCode;
                data.JobName = j.CoreInfo.JobName;
                data.SiteName = j.Site.SiteName;
                data.ClientName = j.Client.ClientName;

                return new ReportDTO<DetailedExpenseForJobReportDTO>()
                {
                    Data = data,
                    ReportName = "Detailed Expense Report",
                    RunSettings = new Dictionary<string, string>()
                    {
                        { "Generated", $"{DateTimeWithZone.EasternStandardTime.ToShortDateString()} at {DateTimeWithZone.EasternStandardTime.ToShortTimeString()}"},
                        { "Company", $"Orion Engineering Co., Inc." },
                    }
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "error creating report");
                throw;
            }
        }


    }

}




