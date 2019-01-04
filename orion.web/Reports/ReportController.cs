using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var vm = await reportSettingsViewModelFactory.GetReportSelectionViewModelAsync();
            return View("ReportsIndex", vm);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.PAY_PERIOD_REPORT)]
        public async Task<ActionResult> PayPeriodReport(ReportSelectionViewModel request)
        {
            var rpt = await reportCreator.CreatePayPeriodReportAsync(request.PayPeriodReport);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(request.PayPeriodReport, "Pay Period Report", rpt);
            return File(Steam, MimeType, Name);
        }


        [HttpPost]
        [Route("RunReport/" + ReportNames.JOBS_SUMMARY_REPORT)]
        public ActionResult JobSummaryReport(ReportSelectionViewModel request)
        {
            var rpt = reportCreator.CreateJobSummaryReport(request.JobSummaryReport);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(request.JobSummaryReport,"Job Summary Report", rpt);
            return File(Steam, MimeType, Name);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOB_DETAIL_REPORT)]
        public ActionResult JobDetailReport(ReportSelectionViewModel request)
        {
            var rpt = reportCreator.CreateJobDetailReport(request.JobDetailReport);
            var (Steam, MimeType, Name) = reportWriter.GetFinishedResult(request.JobDetailReport, "Job Detail Report", rpt);
            return File(Steam, MimeType, Name);
        }
    }
}