using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.Reports
{
    [Authorize]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly IWeekService weekService;
        private readonly IJobService jobService;
        private readonly IReportservice reportservice;

        public ReportsController(IWeekService weekService, IJobService jobService, IReportservice reportservice)
        {
            this.weekService = weekService;
            this.jobService = jobService;
            this.reportservice = reportservice;
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
            var vm = new ReportSelectionViewModel()
            {
                AvailableReports = allReports,
            };
            return View("ReportsIndex", vm);
        }

        [HttpGet]
        [Route("GetReportSetup/{reportName}")]
        public ActionResult GetReportSetup(string reportName)
        {
            var wk = weekService.Get(DateTime.Now);
            var ps = new PeriodBasedReportSettings()
            {
                Start = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Monday),
                End = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Sunday)
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
                    AvailableJobs = jobService.Get().ToList()
                };
                vm.SelectedReport = new ReportNameViewModel()
                {
                    ReportDisplayName = "Job Details Period Report",
                    ReportSystemName = ReportNames.JOB_DETAIL_REPORT
                };
                return View("JobDetailReport", vm);
            }

            throw new NotImplementedException();
        }       

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOBS_SUMMARY_REPORT)]
        public ActionResult JobSummaryReport(ReportViewModel<JobSummaryReportSettings> reportSettings)
        {           
            var rpt = reportservice.GetJobsSummaryReport(reportSettings.Report.PeriodSettings.Start, 
                reportSettings.Report.PeriodSettings.End, 
                reportSettings.Report.ShowAllJobsRegardlessOfHoursBooked,
                reportSettings.SelectedReport.ReportDisplayName);


            if(reportSettings.ForDownload)
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

        [HttpPost]
        [Route("RunReport/" + ReportNames.JOB_DETAIL_REPORT)]
        public ActionResult JobDetailReport(ReportViewModel<JobDetailReport> reportSettings)
        {
            var rpt = reportservice.GetJobDetailReport(reportSettings.Report.PeriodSettings.Start,
                reportSettings.Report.PeriodSettings.End,
                int.Parse(reportSettings.Report.JobBasedReportSettings.SelectedJobId),
                reportSettings.SelectedReport.ReportDisplayName);

            if(reportSettings.ForDownload)
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