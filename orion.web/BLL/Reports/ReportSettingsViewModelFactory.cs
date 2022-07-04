using orion.web.BLL.Reports.AllOpenJobsSummaryreport;
using orion.web.BLL.Reports.DetailedExpenseForJobReport;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.PayPeriod;
using orion.web.Reports.Common;
using orion.web.Reports.EmployeeTimeReport;
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
        private readonly IEmployeeRepository employeeRepository;

        public ReportSettingsViewModelFactory(IJobsRepository jobService, IEmployeeRepository employeeRepository)
        {
            this.jobService = jobService;
            this.employeeRepository = employeeRepository;
        }

        private ExcelReport<PayPeriodReportCriteria> GetPayPeriodReportViewModel(bool isCurrentUserAdmin)
        {
            var vm2 = new PayPeriodReportCriteria();
            vm2.PayPeriodEnd = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekEnd;
            vm2.PayPeriodList = PayPeriodRepository.GetPPRange(rangeSize: 25 );
            var rpt = new ExcelReport<PayPeriodReportCriteria>(PayPeriodReportCriteria.PAY_PERIOD_REPORT_NAME, vm2, isCurrentUserAdmin);
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
                DetailedExpenseForJobReportCriteria = await GetExpenseBreakDownReportCriteria(isCurrentUserAdmin),
                AllOpenJobsSummaryReportCriteria = GetAllOpenJobsReportCriteria(isCurrentUserAdmin),
                EmployeeTimeReportCriteria = await GetEmployeeTimeReportCriteria(isCurrentUserAdmin)
               
            };
            return vm;
        }

        private async Task<ExcelReport<EmployeeTimeReportCriteria>> GetEmployeeTimeReportCriteria(bool isCurrentUserAdmin)
        {
           var vm = new EmployeeTimeReportCriteria();
            vm.PeriodSettings = GetDefaultPeriodSettings();
            var emps = (await employeeRepository.GetAllEmployees()).Where(x => x.EmployeeId != 1).ToList();
            vm.AvailableEmployees = emps.Select(x => new CoreEmployeeDto()
            {
                EmployeeId = x.EmployeeId,
                First = x.First,
                Last = x.Last
            });
            
            var rpt = new ExcelReport<EmployeeTimeReportCriteria>(EmployeeTimeReportCriteria.EMPLOYEE_TIME_REPORT_NAME, vm, isCurrentUserAdmin);
            return rpt;
        }

        private ExcelReport<AllOpenJobsSummaryReportCriteria> GetAllOpenJobsReportCriteria(bool isCurrentUserAdmin)
        {
             var vm = new AllOpenJobsSummaryReportCriteria();
            
            var rpt = new ExcelReport<AllOpenJobsSummaryReportCriteria>(AllOpenJobsSummaryReportCriteria.ALL_OPEN_JOBS_SUMMARY_REPORT, vm, isCurrentUserAdmin);
            return rpt;
        }

        private async Task<ExcelReport<DetailedExpenseForJobReportCriteria>> GetExpenseBreakDownReportCriteria(bool isCurrentUserAdmin)
        {
            var vm = new DetailedExpenseForJobReportCriteria();
            
            vm.AvailableJobs = (await jobService.GetAsync()).OrderBy(x => x.FullJobCodeWithName).ToList();
            var rpt = new ExcelReport<DetailedExpenseForJobReportCriteria>(DetailedExpenseForJobReportCriteria.DETAILED_EXPENSE_REPORT_NAME, vm, isCurrentUserAdmin);
            return rpt;
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
