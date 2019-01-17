using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.ProjectStatusReport;
using orion.web.Reports.QuickJobTimeReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{
    public interface IReportCreator : IRegisterByConvention
    {
        Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings);
        Task<ReportDTO<ProjectStatusReportDTO>> CreateJobSummaryReportAsync(ProjectStatusReportCriteria settings);
        ReportDTO<DataTable> CreateJobDetailReport(QuickJobTimeReportCriteria settings);
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

        public async Task<ReportDTO<PayPeriodReportDTO>> CreatePayPeriodReportAsync(PayPeriodReportCriteria settings)
        {
            return await payPeriodReportQuery.RunAsync(settings.PayPeriodEnd);
        }

        public async Task<ReportDTO<ProjectStatusReportDTO>> CreateJobSummaryReportAsync(ProjectStatusReportCriteria settings)
        {
            return await jobSummaryQuery.RunAsync(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                settings.ShowAllJobsRegardlessOfHoursBooked,
                "Job Summary Report",
                int.Parse(settings.SelectedJobId));
        }

        public ReportDTO<DataTable> CreateJobDetailReport(QuickJobTimeReportCriteria settings)
        {
            return singleJobDetailQuery.Run(settings.PeriodSettings.Start,
                settings.PeriodSettings.End,
                int.Parse(settings.SelectedJobId),
                "Job Detail Report");
        }
    }
}
