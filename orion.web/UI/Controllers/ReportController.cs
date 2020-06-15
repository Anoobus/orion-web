using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Route("RunReport/" + ReportNames.JOBS_SUMMARY_REPORT)]
        public async Task<ActionResult> JobSummaryReport(ReportSelectionViewModel request)
        {
            var criteria = request.JobSummaryReportCriteria.Criteria;
            var rpt = await reportCreator.CreateJobSummaryReportAsync(criteria);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria,  rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOB_DETAIL_REPORT)]
        public async Task<ActionResult> JobDetailReport(ReportSelectionViewModel request)
        {
            var criteria = request.QuickJobTimeReportCriteria.Criteria;
            var rpt = await reportCreator.CreateQuickJobTimeReportAsync(criteria);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(criteria, rpt);
            return File(Steam, MimeType, Name);
        }
    }
}