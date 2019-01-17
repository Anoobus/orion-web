using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using orion.web.Common;
using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IPayPeriodReportQuery
    {
        Task<ReportDTO<PayPeriodReportDTO>> RunAsync(DateTime payPeriodEnd);
    }
    public class PayPeriodReportQuery : IPayPeriodReportQuery
    {
        private readonly IConfiguration configuration;
        private readonly OrionDbContext db;
        private readonly ILogger<PayPeriodReportQuery> logger;

        public PayPeriodReportQuery(IConfiguration configuration, OrionDbContext db, ILogger<PayPeriodReportQuery> logger)
        {
            this.configuration = configuration;
            this.db = db;
            this.logger = logger;
        }
        public async Task<ReportDTO<PayPeriodReportDTO>> RunAsync(DateTime payPeriodEnd)
        {


            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();


                cmd.CommandText = @"
declare @vacationRowId as int
select @vacationRowId = JobTaskId from dbo.JobTasks where [LegacyCode] = '87'

declare @sickRowId as int
select @sickRowId = JobTaskId from dbo.JobTasks where [LegacyCode] = '85'


declare @personalTime as int
select @personalTime = JobTaskId from dbo.JobTasks where [LegacyCode] = '83'

declare @holidayTime as int
select @holidayTime = JobTaskId from dbo.JobTasks where [LegacyCode] = '88'

declare @excusedWithPay as int
select @excusedWithPay = JobTaskId from dbo.JobTasks where [LegacyCode] = '86'

declare @excusedWithoutPay as int
select @excusedWithoutPay = JobTaskId from dbo.JobTasks where [LegacyCode] = '89'

declare @ptoPay as int
select @ptoPay = JobTaskId from dbo.JobTasks where [LegacyCode] = '93'

Select "
+ $"	e.{nameof(PayPeriodEmployees.EmployeeId)}, "
+ $"    e.First + ', ' + e.Last as {nameof(PayPeriodEmployees.EmployeeName)},"
+ $"	IsNull(sum(regular.hours), 0) as {nameof(PayPeriodEmployees.Regular)}, "
+ $"	IsNull(sum(regular.overtimehours), 0) as {nameof(PayPeriodEmployees.Overtime)}, "
+ $"	IsNull(sum(pto.hours) + sum(pto.overtimehours), 0) as {nameof(PayPeriodEmployees.PTO)},"
+ $"	IsNull(sum(holiday.hours) + sum(holiday.overtimehours), 0) as {nameof(PayPeriodEmployees.Holiday)},"
+ $"	IsNull(sum(excusedNoPay.hours) + sum(excusedNoPay.overtimehours), 0) as {nameof(PayPeriodEmployees.ExcusedNoPay)},"
+ $"	IsNull(sum(excusedWithPay.hours) + sum(excusedWithPay.overtimehours), 0) as {nameof(PayPeriodEmployees.ExcusedWithPay)},"
+ $"	e.{nameof(PayPeriodEmployees.IsExempt)},"
+ $"	IsNull(sum(te.hours) + sum(te.overtimehours),0) as {nameof(PayPeriodEmployees.Combined)} "
+
@"from 
[dbo].Employees e
left outer join [dbo].TimeEntries te
	on e.EmployeeId = te.EmployeeId
left outer join dbo.TimeEntries regular
	on te.TimeEntryId = regular.TimeEntryId
	and te.TaskId not in (@ptoPay, @holidayTime, @excusedWithoutPay, @excusedWithPay)
left outer join dbo.TimeEntries pto
	on te.TimeEntryId = pto.TimeEntryId
	and pto.TaskId = @ptoPay
left outer join dbo.TimeEntries holiday
	on te.TimeEntryId = holiday.TimeEntryId
	and holiday.TaskId = @holidayTime
left outer join dbo.TimeEntries excusedNoPay
	on te.TimeEntryId = excusedNoPay.TimeEntryId
	and excusedNoPay.TaskId = @excusedWithoutPay
left outer join dbo.TimeEntries excusedWithPay
	on te.TimeEntryId = excusedWithPay.TimeEntryId
	and excusedWithPay.TaskId = @excusedWithPay
where 
    (te.Date >= @payPeriodStart and te.Date <= @payPeriodEnd)
group by e.EmployeeId, 
	e.First + ', ' + e.Last,
	e.IsExempt
	  ";

                var end = WeekDTO.CreateWithWeekContaining(payPeriodEnd);
                var ppe = end.WeekEnd;
                var pps = end.Previous().WeekStart;
                cmd.Parameters.Add(new SqlParameter("payPeriodStart", end.Previous().WeekStart));
                cmd.Parameters.Add(new SqlParameter("payPeriodEnd", end.WeekEnd));

                var data = new PayPeriodReportDTO()
                {
                    PayPeriodEnd = ppe,
                    PayPeriodState = pps
                };
                var emps = new List<PayPeriodEmployees>();
                try
                {

                var rdr = await cmd.ExecuteReaderAsync();
                var map = GetColumnMap(rdr.GetColumnSchema());
                while (await rdr.ReadAsync())
                {
                    emps.Add(new PayPeriodEmployees()
                    {
                        Combined = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Combined)]),
                        EmployeeName = rdr.IsDBNull(map[nameof(PayPeriodEmployees.EmployeeName)]) ? "" : rdr.GetSqlString(map[nameof(PayPeriodEmployees.EmployeeName)]).Value,
                        ExcusedNoPay = rdr.GetDecimal(map[nameof(PayPeriodEmployees.ExcusedNoPay)]),
                        ExcusedWithPay = rdr.GetDecimal(map[nameof(PayPeriodEmployees.ExcusedWithPay)]),
                        Holiday = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Holiday)]),
                        IsExempt = rdr.GetBoolean(map[nameof(PayPeriodEmployees.IsExempt)]),
                        Overtime = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Overtime)]),
                        Regular = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Regular)]),
                        PTO = rdr.GetDecimal(map[nameof(PayPeriodEmployees.PTO)]),
                    });
                }
                data.Employees = emps;
                return new ReportDTO<PayPeriodReportDTO>()
                {
                    Data = data,
                    ReportName = "Pay Period Report",
                    RunSettings =
                new Dictionary<string, string>()
                {
                    { "Pay Period", $"{pps.ToShortDateString()} thru {ppe.ToShortDateString()}" },
                    { "Generated", $"{DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}"},
                    { "Company", $"Orion Engineering Co., Inc." },
                }
                };
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"Error while running {cmd.CommandText}");
                    throw;
                }
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
                    var empPropNames = typeof(PayPeriodEmployees).GetProperties();
                    foreach (var prop in empPropNames)
                    {
                        ColumnMap.Add(prop.Name, cols.Single(x => x.ColumnName == prop.Name).ColumnOrdinal.Value);
                    }
                }

            }
            return ColumnMap;
        }



    }
}
