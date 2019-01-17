using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.Common;
using orion.web.Reports.PayPeriodReport;
using orion.web.Reports.ProjectStatusReport;
using orion.web.Reports.QuickJobTimeReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports
{

    public interface IReportSettingsViewModelFactory : IRegisterByConvention
    {
        Task<ReportSelectionViewModel> GetReportSelectionViewModelAsync(bool isCurrentUserAdmin);
    }

    public class ReportSettingsViewModelFactory : IReportSettingsViewModelFactory
    {
        private readonly IJobService jobService;

        public ReportSettingsViewModelFactory(IJobService jobService)
        {
            this.jobService = jobService;
        }

        private ExcelReport<PayPeriodReportCriteria> GetPayPeriodReportViewModel(bool isCurrentUserAdmin)
        {
            var vm2 = new PayPeriodReportCriteria();
            vm2.PayPeriodEnd = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekEnd;
            var rpt = new ExcelReport<PayPeriodReportCriteria>(PayPeriodReportCriteria.PAY_PERIOD_REPORT_NAME, vm2, isCurrentUserAdmin);
            return rpt;
        }

        private async Task<ExcelReport<ProjectStatusReportCriteria>> GetProjectStatusReportCriteria(bool isCurrentUserAdmin)
        {
            var vm = new ProjectStatusReportCriteria();
            vm.PeriodSettings = GetDefaultPeriodSettings();
            vm.AvailableJobs = (await jobService.GetAsync()).OrderBy(x => x.FullJobCodeWithName).ToList();
            var rpt = new ExcelReport<ProjectStatusReportCriteria>(ProjectStatusReportCriteria.PROJECT_STATUS_REPORT_NAME, vm, isCurrentUserAdmin);
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
                ProjectStatusReportCriteria = await GetProjectStatusReportCriteria(false)
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
