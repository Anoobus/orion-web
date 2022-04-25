using orion.web.BLL.Reports.DetailedExpenseForJobReport;
using orion.web.Jobs;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.QuickJobTimeReport;
using orion.web.Util.IoC;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportCreator
    {
        Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings);
        Task<ReportDTO<AllOpenJobSummaryReportDTO>> CreateAllOpenJobSummaryReportAsync();
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

        public ReportCreator(IJobsRepository jobService,
            IAllOpenJobSummaryReportQuery jobSummaryQuery,
            IQuickJobTimeReportQuery quickJobTimeReportQuery,
            IPayPeriodReportQuery payPeriodReportQuery,
            IDetailedExpenseForJobReportQuery detailedExpenseForJobReportQuery
            )
        {
            this.jobService = jobService;
            this.jobSummaryQuery = jobSummaryQuery;
            this.quickJobTimeReportQuery = quickJobTimeReportQuery;
            this.payPeriodReportQuery = payPeriodReportQuery;
            this.detailedExpenseForJobReportQuery = detailedExpenseForJobReportQuery;
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
    }
}
