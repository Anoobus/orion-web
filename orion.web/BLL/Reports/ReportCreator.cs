using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.JobSummaryReport;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.QuickJobTimeReport;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportCreator : IRegisterByConvention
    {
        Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings);
        Task<ReportDTO<JobSummaryReportDTO>> CreateJobSummaryReportAsync(JobSummaryReportCriteria settings);
        Task<ReportDTO<QuickJobTimeReportDTO>> CreateQuickJobTimeReportAsync(QuickJobTimeReportCriteria settings);
    }

    public class ReportCreator : IReportCreator
    {
        private readonly IJobService jobService;
        private readonly IJobSummaryReportQuery jobSummaryQuery;
        private readonly IQuickJobTimeReportQuery quickJobTimeReportQuery;
        private readonly IPayPeriodReportQuery payPeriodReportQuery;

        public ReportCreator(IJobService jobService, IJobSummaryReportQuery jobSummaryQuery, IQuickJobTimeReportQuery quickJobTimeReportQuery, IPayPeriodReportQuery payPeriodReportQuery)
        {
            this.jobService = jobService;
            this.jobSummaryQuery = jobSummaryQuery;
            this.quickJobTimeReportQuery = quickJobTimeReportQuery;
            this.payPeriodReportQuery = payPeriodReportQuery;
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
    }
}
