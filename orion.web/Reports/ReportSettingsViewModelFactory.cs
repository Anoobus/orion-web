using orion.web.Common;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{

    public interface IReportSettingsViewModelFactory : IRegisterByConvention
    {
        Task<ReportSelectionViewModel> GetReportSelectionViewModelAsync();
    }

    public class ReportSettingsViewModelFactory : IReportSettingsViewModelFactory
    {
        private readonly IJobService jobService;

        public ReportSettingsViewModelFactory(IJobService jobService)
        {
            this.jobService = jobService;
        }
        private PayPeriodReport GetPayPeriodReportViewModel()
        {
            var vm2 = new PayPeriodReport();
            vm2.PayPeriodEnd = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekEnd;          
            return vm2;
        }

        private JobSummaryReportSettings GetJobSummaryreport()
        {
            var vm = new JobSummaryReportSettings();
            vm.PeriodSettings = GetDefaultPeriodSettings();           
            return vm;
        }

        private async Task<JobDetailReport> GetJobDetailreportAsync()
        {
            var vm = new JobDetailReport();
            vm.PeriodSettings = GetDefaultPeriodSettings();
            vm.AvailableJobs = (await jobService.GetAsync()).ToList();
            return vm;
        }
        public async Task<ReportSelectionViewModel> GetReportSelectionViewModelAsync()
        {
            var allReports = new List<string>()
            {
                "All Jobs For Period Report",
                 "Job Details Period Report",
                "Pay Period Report"
            };
          
            var vm = new ReportSelectionViewModel()
            {
                AvailableReports = allReports,
                PayPeriodReport = GetPayPeriodReportViewModel(),
                JobDetailReport = await GetJobDetailreportAsync(),
                JobSummaryReport = GetJobSummaryreport()
            };
            return vm;
        }
        private PeriodBasedReportSettings GetDefaultPeriodSettings()
        {
            var wk = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            var ps = new PeriodBasedReportSettings()
            {
                Start = wk.WeekStart,
                End = wk.WeekEnd
            };
            return ps;
        }
    }
}
