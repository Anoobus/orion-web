using Microsoft.Extensions.Configuration;
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
        Task<ReportDTO<PayPeriodDataDTO>> RunAsync(DateTime payPeriodEnd);
    }
    public class PayPeriodReportQuery : IPayPeriodReportQuery
    {
        private readonly IConfiguration configuration;
        private readonly OrionDbContext db;

        public PayPeriodReportQuery(IConfiguration configuration, OrionDbContext db)
        {
            this.configuration = configuration;
            this.db = db;
        }
        public async Task<ReportDTO<PayPeriodDataDTO>> RunAsync(DateTime payPeriodEnd)
        {


            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();


                cmd.CommandText = @"
declare @vacationRowId as int
select @vacationRowId = JobTaskId from dbo.JobTasks where ShortName = '87 - Vacation'

declare @sickRowId as int
select @sickRowId = JobTaskId from dbo.JobTasks where ShortName = '85 - Sick Time'


declare @personalTime as int
select @personalTime = JobTaskId from dbo.JobTasks where ShortName = '83 - Personal Time'

declare @holidayTime as int
select @holidayTime = JobTaskId from dbo.JobTasks where ShortName = '88 - Holiday'

declare @excusedWithoutPay as int
select @excusedWithoutPay = JobTaskId from dbo.JobTasks where ShortName = '89 - Excused WITHOUT Pay'


Select "
+ $"	e.{nameof(PayPeriodEmployees.EmployeeId)}, "
+ $"    e.First + ', ' + e.Last as {nameof(PayPeriodEmployees.EmployeeName)},"
+ $"	IsNull(sum(regular.hours), 0) as {nameof(PayPeriodEmployees.Regular)}, "
+ $"	IsNull(sum(regular.overtimehours), 0) as {nameof(PayPeriodEmployees.Overtime)}, "
+ $"	IsNull(sum(vacation.hours) + sum(vacation.overtimehours), 0) as {nameof(PayPeriodEmployees.Vacation)},"
+ $"	IsNull(sum(sick.hours) + sum(sick.overtimehours), 0) as {nameof(PayPeriodEmployees.Sick)},"
+ $"	IsNull(sum(personal.hours) + sum(personal.overtimehours), 0) as {nameof(PayPeriodEmployees.Personal)},"
+ $"	IsNull(sum(holiday.hours) + sum(holiday.overtimehours), 0) as {nameof(PayPeriodEmployees.Holiday)},"
+ $"	IsNull(sum(excusedNoPay.hours) + sum(excusedNoPay.overtimehours), 0) as {nameof(PayPeriodEmployees.ExcusedNoPay)},"
+ $"	e.{nameof(PayPeriodEmployees.IsExempt)},"
+ $"	IsNull(sum(te.hours) + sum(te.overtimehours),0) as {nameof(PayPeriodEmployees.Combined)} "
+
@"from 
[dbo].Employees e
left outer join [dbo].TimeEntries te
	on e.EmployeeId = te.EmployeeId
left outer join dbo.TimeEntries regular
	on te.TimeEntryId = regular.TimeEntryId
	and te.TaskId not in (@vacationRowId, @sickRowId, @personalTime, @holidayTime, @excusedWithoutPay)
left outer join dbo.TimeEntries vacation
	on te.TimeEntryId = vacation.TimeEntryId
	and vacation.TaskId = @vacationRowId
left outer join dbo.TimeEntries sick
	on te.TimeEntryId = sick.TimeEntryId
	and sick.TaskId = @sickRowId
left outer join dbo.TimeEntries personal
	on te.TimeEntryId = personal.TimeEntryId
	and personal.TaskId = @personalTime
left outer join dbo.TimeEntries holiday
	on te.TimeEntryId = holiday.TimeEntryId
	and holiday.TaskId = @holidayTime
left outer join dbo.TimeEntries excusedNoPay
	on te.TimeEntryId = excusedNoPay.TimeEntryId
	and excusedNoPay.TaskId = @excusedWithoutPay	
where 
    te.Date is null OR
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

                var data = new PayPeriodDataDTO()
                {
                    PayPeriodEnd = ppe,
                    PayPeriodState = pps
                };
                var emps = new List<PayPeriodEmployees>();
                var rdr = await cmd.ExecuteReaderAsync();
                var map = GetColumnMap(rdr.GetColumnSchema());
                while (await rdr.ReadAsync())
                {
                    emps.Add(new PayPeriodEmployees()
                    {
                        Combined = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Combined)]),
                        EmployeeName = rdr.GetSqlString(map[nameof(PayPeriodEmployees.EmployeeName)]).Value,
                        ExcusedNoPay = rdr.GetDecimal(map[nameof(PayPeriodEmployees.ExcusedNoPay)]),
                        Holiday = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Holiday)]),
                        IsExempt = rdr.GetBoolean(map[nameof(PayPeriodEmployees.IsExempt)]),
                        Overtime = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Overtime)]),
                        Personal = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Personal)]),
                        Regular = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Regular)]),
                        Sick = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Sick)]),
                        Vacation = rdr.GetDecimal(map[nameof(PayPeriodEmployees.Vacation)]),
                    });
                }
                data.Employees = emps;
                return new ReportDTO<PayPeriodDataDTO>()
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
