using System.Threading.Tasks;
using Orion.Web.BLL.Reports.DetailedExpenseForJobReport;
using Orion.Web.Jobs;
using Orion.Web.Reports.EmployeeTimeReport;
using Orion.Web.Reports.PayPeriodReport;
using Orion.Web.Reports.QuickJobTimeReport;
using Orion.Web.Util.IoC;

namespace Orion.Web.Reports
{
    public interface IReportCreator
    {
        Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings);
        Task<ReportDTO<AllOpenJobSummaryReportDTO>> CreateAllOpenJobSummaryReportAsync();
        Task<ReportDTO<EmployeeTimeReportDTO>> CreateEmployeeTimeReportAsync(EmployeeTimeReportCriteria settings);
        Task<ReportDTO<QuickJobTimeReportDTO>> CreateQuickJobTimeReportAsync(QuickJobTimeReportCriteria settings);
        Task<ReportDTO<DetailedExpenseForJobReportDTO>> CreateDetailedExpenseReport(DetailedExpenseForJobReportCriteria criteria);
    }

    public class ReportCreator : IReportCreator, IAutoRegisterAsSingleton
    {
        private readonly IJobsRepository jobService;
        private readonly IAllOpenJobSummaryReportQuery jobSummaryQuery;
        private readonly IQuickJobTimeReportQuery quickJobTimeReportQuery;
        private readonly IPayPeriodReportQuery payPeriodReportQuery;
        private readonly IDetailedExpenseForJobReportQuery detailedExpenseForJobReportQuery;
        private readonly IEmployeeTimeReportQuery employeeTimeReportQuery;

        public ReportCreator(
            IJobsRepository jobService,
            IAllOpenJobSummaryReportQuery jobSummaryQuery,
            IQuickJobTimeReportQuery quickJobTimeReportQuery,
            IPayPeriodReportQuery payPeriodReportQuery,
            IDetailedExpenseForJobReportQuery detailedExpenseForJobReportQuery,
            IEmployeeTimeReportQuery employeeTimeReportQuery
            )
        {
            this.jobService = jobService;
            this.jobSummaryQuery = jobSummaryQuery;
            this.quickJobTimeReportQuery = quickJobTimeReportQuery;
            this.payPeriodReportQuery = payPeriodReportQuery;
            this.detailedExpenseForJobReportQuery = detailedExpenseForJobReportQuery;
            this.employeeTimeReportQuery = employeeTimeReportQuery;
        }

        public async Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings)
        {
            return await payPeriodReportQuery.RunAsync(settings.PayPeriodEnd);
        }

        public async Task<ReportDTO<AllOpenJobSummaryReportDTO>> CreateAllOpenJobSummaryReportAsync()
        {
            return await jobSummaryQuery.RunAsync(true, "All Open Job Summary Report");
        }

        public async Task<ReportDTO<QuickJobTimeReportDTO>> CreateQuickJobTimeReportAsync(QuickJobTimeReportCriteria settings)
        {
            return await quickJobTimeReportQuery.RunAsync(settings);
        }

        public async Task<ReportDTO<DetailedExpenseForJobReportDTO>> CreateDetailedExpenseReport(DetailedExpenseForJobReportCriteria criteria)
        {
            return await detailedExpenseForJobReportQuery.RunAsync(criteria);
        }

        public async Task<ReportDTO<EmployeeTimeReportDTO>> CreateEmployeeTimeReportAsync(EmployeeTimeReportCriteria settings)
        {
            return await employeeTimeReportQuery.RunAsync(settings);
        }
    }
}
