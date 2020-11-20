using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
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
    public class PayPeriodReportQuery : IPayPeriodReportQuery, IAutoRegisterAsSingleton
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<PayPeriodReportQuery> logger;

        public PayPeriodReportQuery(IConfiguration configuration, ILogger<PayPeriodReportQuery> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task<ReportDTO<PayPeriodReportDTO>> RunAsync(DateTime payPeriodEnd)
        {


            using (var conn = new SqlConnection(configuration.GetConnectionString("SiteConnection")))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();


                cmd.CommandText = @"

declare @regularReportingType as int = 0
declare @PtoReportingType as int = 1
declare @HolidayReportingType as int = 2
declare @ExcusedWithPayReportingType as int = 3
declare @ExcusedWithOutPayReportingType as int = 4

Select "
+ $"	e.{nameof(PayPeriodEmployees.EmployeeId)}, "
+ $"    COALESCE(e.Last,'') + ', ' + COALESCE(e.First,'') as {nameof(PayPeriodEmployees.EmployeeName)},"
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
left outer join [dbo].JobTasks jt
	on jt.JobTaskId = te.TaskId
left outer join dbo.TimeEntries regular
	on te.TimeEntryId = regular.TimeEntryId
	and jt.[ReportingClassificationId] = @regularReportingType
left outer join dbo.TimeEntries pto
	on te.TimeEntryId = pto.TimeEntryId
	and jt.[ReportingClassificationId] = @PtoReportingType
left outer join dbo.TimeEntries holiday
	on te.TimeEntryId = holiday.TimeEntryId
	and jt.[ReportingClassificationId] = @HolidayReportingType
left outer join dbo.TimeEntries excusedNoPay
	on te.TimeEntryId = excusedNoPay.TimeEntryId
	and jt.[ReportingClassificationId] = @ExcusedWithOutPayReportingType
left outer join dbo.TimeEntries excusedWithPay
	on te.TimeEntryId = excusedWithPay.TimeEntryId
	and jt.[ReportingClassificationId] = @ExcusedWithPayReportingType
where 
    (te.Date >= @payPeriodStart and te.Date <= @payPeriodEnd)
    and ISNULL(e.[UserName],'') != 'admin@company.com'
group by e.EmployeeId, 
	COALESCE(e.Last,'') + ', ' + COALESCE(e.First,''),
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
                    { "Check Date/Pay Date", $"{ppe.AddDays(7).ToShortDateString()}" },
                    { "Generated", $"{DateTimeWithZone.EasternStandardTime.ToShortDateString()} at {DateTimeWithZone.EasternStandardTime.ToShortTimeString()}"},
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
