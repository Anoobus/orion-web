using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using orion.web.Employees;
using System.Threading.Tasks;

namespace orion.web.Reports
{


    [Authorize]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly IReportSettingsViewModelFactory reportSettingsViewModelFactory;
        private readonly IReportCreator reportCreator;
        private readonly IReportWriter reportWriter;


        public ReportsController(IReportSettingsViewModelFactory reportSettingsViewModelFactory, IReportCreator reportCreator, IReportWriter reportWriter)
        {
            this.reportSettingsViewModelFactory = reportSettingsViewModelFactory;
            this.reportCreator = reportCreator;
            this.reportWriter = reportWriter;

        }

        public async Task<ActionResult> Index()
        {
            var vm = await reportSettingsViewModelFactory.GetReportSelectionViewModelAsync(User.IsInRole(UserRoleName.Admin));
            return View("ReportsIndex", vm);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.PAY_PERIOD_REPORT)]
        public async Task<ActionResult> PayPeriodReport(ReportSelectionViewModel request)
        {
            var criteria = request.PayPeriodReportCriteria.Criteria;
            var rpt = await reportCreator.CreatePayPeriodReportAsync(criteria);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria,  rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.DETAILED_EXPENSE_REPORT)]
        public async Task<ActionResult> DetailedExpenseReport(ReportSelectionViewModel request)
        {
            var criteria = request.DetailedExpenseForJobReportCriteria.Criteria;
            var rpt = await reportCreator.CreateDetailedExpenseReport(criteria);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria,  rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.ALL_OPEN_JOBS_SUMMARY_REPORT)]
        public async Task<ActionResult> AllOpenJobSummaryReport(ReportSelectionViewModel request)
        {            
            var rpt = await reportCreator.CreateAllOpenJobSummaryReportAsync();
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.EMPLOYEE_TIME_REPORT)]
        public async Task<ActionResult> EmployeeTimeReport(ReportSelectionViewModel request)
        {
            var criteria = request.EmployeeTimeReportCriteria.Criteria;
            var rpt = await reportCreator.CreateEmployeeTimeReportAsync(criteria);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria, rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOB_DETAIL_REPORT)]
        public async Task<ActionResult> JobDetailReport(ReportSelectionViewModel request)
        {
            try
            {
                var criteria = request.QuickJobTimeReportCriteria.Criteria;
                var rpt = await reportCreator.CreateQuickJobTimeReportAsync(criteria);
                var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria, rpt);
                return File(Steam, MimeType, Name);
            }
            catch(System.Exception e)
            {
                Serilog.Log.Error(e, $"Error while handleing {ReportNames.JOB_DETAIL_REPORT}");
                throw;
            }

        }
    }
}