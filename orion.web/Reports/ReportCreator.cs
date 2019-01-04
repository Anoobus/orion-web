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
        ReportDTO<DataTable> CreateJobSummaryReport(JobSummaryReportSettings settings);
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

        public ReportDTO<DataTable> CreateJobSummaryReport(JobSummaryReportSettings settings)
        {
            return jobSummaryQuery.Run(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                settings.ShowAllJobsRegardlessOfHoursBooked,
                "Job Summary Report");
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
