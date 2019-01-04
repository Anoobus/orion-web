using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace orion.web.Reports
{
    [Authorize]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly IJobService jobService;
        private readonly IJobSummaryQuery jobSummaryQuery;
        private readonly ISingleJobDetailQuery singleJobDetailQuery;
        private readonly IPayPeriodReportQuery payPeriodReportQuery;

        public ReportsController(IJobService jobService, IJobSummaryQuery jobSummaryQuery, ISingleJobDetailQuery singleJobDetailQuery, IPayPeriodReportQuery payPeriodReportQuery)
        {
            this.jobService = jobService;
            this.jobSummaryQuery = jobSummaryQuery;
            this.singleJobDetailQuery = singleJobDetailQuery;
            this.payPeriodReportQuery = payPeriodReportQuery;
        }

        public ActionResult Index()
        {
            var allReports = new List<ReportNameViewModel>();
            allReports.Add(new ReportNameViewModel()
            {
                ReportDisplayName = "All Jobs For Period Report",
                ReportSystemName = ReportNames.JOBS_SUMMARY_REPORT
            });

            allReports.Add(new ReportNameViewModel()
            {
                ReportDisplayName = "Job Details Period Report",
                ReportSystemName = ReportNames.JOB_DETAIL_REPORT
            });

            allReports.Add(new ReportNameViewModel()
            {
                ReportDisplayName = "Pay Period Report",
                ReportSystemName = ReportNames.PAY_PERIOD_REPORT
            });
            var vm = new ReportSelectionViewModel()
            {
                AvailableReports = allReports,
            };
            return View("ReportsIndex", vm);
        }

        [HttpGet]
        [Route("GetReportSetup/{reportName}")]
        public async System.Threading.Tasks.Task<ActionResult> GetReportSetup(string reportName)
        {
            var wk = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            var ps = new PeriodBasedReportSettings()
            {
                Start = wk.WeekStart,
                End = wk.WeekEnd
            };
            if(reportName == ReportNames.JOBS_SUMMARY_REPORT)
            {
                var vm = new ReportViewModel<JobSummaryReportSettings>();
                vm.Report.PeriodSettings = ps;
                vm.SelectedReport = new ReportNameViewModel()
                {
                    ReportDisplayName = "All Jobs For Period Report",
                    ReportSystemName = ReportNames.JOBS_SUMMARY_REPORT
                };
                return View("JobSummaryReport", vm);
            }
            else if(reportName == ReportNames.JOB_DETAIL_REPORT)
            {
                var vm = new ReportViewModel<JobDetailReport>();
                vm.Report.PeriodSettings = ps;
                vm.Report.JobBasedReportSettings = new JobBasedReportSettings()
                {
                    AvailableJobs = (await jobService.GetAsync()).ToList()
                };
                vm.SelectedReport = new ReportNameViewModel()
                {
                    ReportDisplayName = "Job Details Period Report",
                    ReportSystemName = ReportNames.JOB_DETAIL_REPORT
                };
                return View("JobDetailReport", vm);
            }
            else if (reportName == ReportNames.PAY_PERIOD_REPORT)
            {
                var vm = new ReportViewModel<PayPeriodReport>();
                vm.Report.PayPeriodEnd = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekEnd;
                vm.SelectedReport = new ReportNameViewModel()
                {
                    ReportDisplayName = "Pay Period Report",
                    ReportSystemName = ReportNames.PAY_PERIOD_REPORT
                };

                return View("PayPeriodReport", vm);
            }

            throw new NotImplementedException();
        }
        

        [HttpPost]
        [Route("RunReport/" + ReportNames.PAY_PERIOD_REPORT)]
        public async System.Threading.Tasks.Task<ActionResult> PayPeriodReport(ReportViewModel<PayPeriodReport> reportSettings)
        {
            
            var rpt = await payPeriodReportQuery.RunAsync(reportSettings.Report.PayPeriodEnd,reportSettings.SelectedReport.ReportDisplayName);

            return GetFinishedResult(reportSettings, rpt);
        }

        private ActionResult GetFinishedResult<T>(ReportViewModel<T> reportSettings, ReportDTO<PayPeriodDataDTO> rpt) where T : new()
        {
            if (reportSettings.ForDownload)
            {
                var export = new PayPeriodExcelExport();
                var memoryStream = export.AsXls(rpt);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportSettings.SelectedReport.ReportDisplayName}.xlsx");
            }
            else
            {
                return View("ViewCompletedReport", rpt);
            }
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOBS_SUMMARY_REPORT)]
        public ActionResult JobSummaryReport(ReportViewModel<JobSummaryReportSettings> reportSettings)
        {           
            var rpt = jobSummaryQuery.Run(reportSettings.Report.PeriodSettings.Start, 
                reportSettings.Report.PeriodSettings.End, 
                reportSettings.Report.ShowAllJobsRegardlessOfHoursBooked,
                reportSettings.SelectedReport.ReportDisplayName);

            return GetFinishedResultOther(reportSettings, rpt);
        }

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOB_DETAIL_REPORT)]
        public ActionResult JobDetailReport(ReportViewModel<JobDetailReport> reportSettings)
        {
            var rpt = singleJobDetailQuery.Run(reportSettings.Report.PeriodSettings.Start,
                reportSettings.Report.PeriodSettings.End,
                int.Parse(reportSettings.Report.JobBasedReportSettings.SelectedJobId),
                reportSettings.SelectedReport.ReportDisplayName);

            return GetFinishedResultOther(reportSettings, rpt);
        }

        private ActionResult GetFinishedResultOther<T>(ReportViewModel<T> reportSettings, ReportDTO<DataTable> rpt) where T : new()
        {
            if (reportSettings.ForDownload)
            {
                var export = new ExcelExport();
                var memoryStream = export.AsXls(rpt);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportSettings.SelectedReport.ReportDisplayName}.xlsx");
            }
            else
            {
                return View("ViewCompletedReport", rpt);
            }
        }


    }
}