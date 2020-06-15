using orion.web.Common;
using orion.web.Jobs;
using orion.web.PayPeriod;
using orion.web.Reports.Common;
using orion.web.Reports.JobSummaryReport;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.QuickJobTimeReport;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{

    public interface IReportSettingsViewModelFactory
    {
        Task<ReportSelectionViewModel> GetReportSelectionViewModelAsync(bool isCurrentUserAdmin);
    }

    public class ReportSettingsViewModelFactory : IReportSettingsViewModelFactory, IAutoRegisterAsSingleton
    {
        private readonly IJobsRepository jobService;

        public ReportSettingsViewModelFactory(IJobsRepository jobService)
        {
            this.jobService = jobService;
        }

        private ExcelReport<PayPeriodReportCriteria> GetPayPeriodReportViewModel(bool isCurrentUserAdmin)
        {
            var vm2 = new PayPeriodReportCriteria();
            vm2.PayPeriodEnd = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekEnd;
            vm2.PayPeriodList = PayPeriodRepository.GetPPRange(rangeSize: 25 );
            var rpt = new ExcelReport<PayPeriodReportCriteria>(PayPeriodReportCriteria.PAY_PERIOD_REPORT_NAME, vm2, isCurrentUserAdmin);
            return rpt;
        }

        private async Task<ExcelReport<JobSummaryReportCriteria>> GetJobSummaryReportCriteria(bool isCurrentUserAdmin)
        {
            var vm = new JobSummaryReportCriteria();
            vm.PeriodSettings = GetDefaultPeriodSettings();
            vm.AvailableJobs = (await jobService.GetAsync()).OrderBy(x => x.FullJobCodeWithName).ToList();
            var rpt = new ExcelReport<JobSummaryReportCriteria>(JobSummaryReportCriteria.PROJECT_STATUS_REPORT_NAME, vm, isCurrentUserAdmin);
            return rpt;
        }

        private async Task<ExcelReport<QuickJobTimeReportCriteria>> GetJobDetailreportAsync()
        {
            var vm = new QuickJobTimeReportCriteria();
            vm.PeriodSettings = GetDefaultPeriodSettings();
            vm.AvailableJobs = (await jobService.GetAsync()).OrderBy(x => x.FullJobCodeWithName).ToList();
            var rpt = new ExcelReport<QuickJobTimeReportCriteria>(QuickJobTimeReportCriteria.QUICK_JOB_TIME_REPORT_NAME, vm, true);
            return rpt;
        }
        public async Task<ReportSelectionViewModel> GetReportSelectionViewModelAsync(bool isCurrentUserAdmin)
        {

            var vm = new ReportSelectionViewModel()
            {
                PayPeriodReportCriteria = GetPayPeriodReportViewModel(isCurrentUserAdmin),
                QuickJobTimeReportCriteria = await GetJobDetailreportAsync(),
                JobSummaryReportCriteria = await GetJobSummaryReportCriteria(false)
            };
            return vm;
        }
        private ReportingPeriod GetDefaultPeriodSettings()
        {
            var wk = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            var ps = new ReportingPeriod()
            {
                Start = wk.WeekStart,
                End = wk.WeekEnd
            };
            return ps;
        }
    }
}
