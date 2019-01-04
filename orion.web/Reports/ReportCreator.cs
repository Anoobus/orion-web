using orion.web.Common;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportCreator : IRegisterByConvention
    {
        Task<ReportDTO<PayPeriodDataDTO>> CreatePayPeriodReportAsync(PayPeriodReport settings);
        Task<ReportDTO<JobSummaryReportDataDTO>> CreateJobSummaryReportAsync(JobSummaryReportSettings settings);
        ReportDTO<DataTable> CreateJobDetailReport(JobDetailReport settings);
    }

    public class ReportCreator : IReportCreator
    {
        private readonly IJobService jobService;
        private readonly IJobSummaryQuery jobSummaryQuery;
        private readonly ISingleJobDetailQuery singleJobDetailQuery;
        private readonly IPayPeriodReportQuery payPeriodReportQuery;

        public ReportCreator(IJobService jobService, IJobSummaryQuery jobSummaryQuery, ISingleJobDetailQuery singleJobDetailQuery, IPayPeriodReportQuery payPeriodReportQuery)
        {
            this.jobService = jobService;
            this.jobSummaryQuery = jobSummaryQuery;
            this.singleJobDetailQuery = singleJobDetailQuery;
            this.payPeriodReportQuery = payPeriodReportQuery;
        }

        public async Task<ReportDTO<PayPeriodDataDTO>> CreatePayPeriodReportAsync(PayPeriodReport settings)
        {
            return await payPeriodReportQuery.RunAsync(settings.PayPeriodEnd);
        }

        public async Task<ReportDTO<JobSummaryReportDataDTO>> CreateJobSummaryReportAsync(JobSummaryReportSettings settings)
        {
            return await jobSummaryQuery.RunAsync(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                settings.ShowAllJobsRegardlessOfHoursBooked,
                "Job Summary Report",
                int.Parse(settings.SelectedJobId));
        }

        public ReportDTO<DataTable> CreateJobDetailReport(JobDetailReport settings)
        {
            return singleJobDetailQuery.Run(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                int.Parse(settings.SelectedJobId),
                "Job Detail Report");
        }
    }
}
