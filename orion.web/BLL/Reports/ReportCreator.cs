using orion.web.BLL.Reports.DetailedExpenseForJobReport;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.JobSummaryReport;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.QuickJobTimeReport;
using orion.web.Util.IoC;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportCreator
    {
        Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings);
        Task<ReportDTO<JobSummaryReportDTO>> CreateJobSummaryReportAsync(JobSummaryReportCriteria settings);
        Task<ReportDTO<QuickJobTimeReportDTO>> CreateQuickJobTimeReportAsync(QuickJobTimeReportCriteria settings);
        Task<ReportDTO<DetailedExpenseForJobReportDTO>> CreateDetailedExpenseReport(DetailedExpenseForJobReportCriteria criteria);
    }

    public class ReportCreator : IReportCreator, IAutoRegisterAsSingleton
    {
        private readonly IJobsRepository jobService;
        private readonly IJobSummaryReportQuery jobSummaryQuery;
        private readonly IQuickJobTimeReportQuery quickJobTimeReportQuery;
        private readonly IPayPeriodReportQuery payPeriodReportQuery;
        private readonly IDetailedExpenseForJobReportQuery detailedExpenseForJobReportQuery;

        public ReportCreator(IJobsRepository jobService,
            IJobSummaryReportQuery jobSummaryQuery,
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

        public async Task<ReportDTO<JobSummaryReportDTO>> CreateJobSummaryReportAsync(JobSummaryReportCriteria settings)
        {
            return await jobSummaryQuery.RunAsync(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                settings.ShowAllJobsRegardlessOfHoursBooked,
                "Job Summary Report",
                int.Parse(settings.SelectedJobId));
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
