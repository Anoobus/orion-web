using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using orion.web.Common;
using orion.web.Jobs;

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
            return View("Index");
        }

        [HttpGet]
        [Route("RunReport/{reportName}")]
        public ActionResult RunReport(string reportName)
        {
            var vm = new ReportViewModel();
            var wk = weekService.Get(DateTime.Now);
            vm.Start = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Monday);
            vm.End = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Sunday);
            vm.AvailableJobs = jobService.Get().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.FullJobCodeWithName, Value = x.JobId.ToString() }).ToArray();
            vm.ReportName = reportName;
            return View("ReportDetails", vm);
        }

        [HttpPost]
        [Route("RunReport/{reportName}/xlsx")]
        public ActionResult RunReportXlsx(string reportName, ReportViewModel model)
        {
            var vm = new ReportViewModel();
            var wk = weekService.Get(DateTime.Now);
            vm.Start = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Monday);
            vm.End = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Sunday);
            vm.AvailableJobs = jobService.Get().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.FullJobCodeWithName, Value = x.JobId.ToString() }).ToArray();
            vm.ReportName = model.ReportName;
            int? joby = null;
            if(int.TryParse(model.AvailableJobs.FirstOrDefault(x => x.Selected)?.Value, out var jobId))
            {
                joby = jobId;
            }
            var rpt = reportservice.AsXls(reportName, model.Start, model.End, joby);
            return File(rpt, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
        }
        [HttpPost]
        [Route("RunReport/{reportName}")]
        public ActionResult RunReport(string reportName, ReportViewModel model)
        {
            var vm = new ReportViewModel();
            var wk = weekService.Get(DateTime.Now);
            vm.Start = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Monday);
            vm.End = weekService.GetWeekDate(wk.Year, wk.WeekId, DayOfWeek.Sunday);
            vm.AvailableJobs = jobService.Get().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.FullJobCodeWithName, Value = x.JobId.ToString() }).ToArray();
            vm.ReportName = model.ReportName;
            int? joby = null;
            if(model.SelectedJobId != "all")
            {
                if(int.TryParse(model.SelectedJobId, out var jobId))
                {
                    joby = jobId;
                }
            }

            if(model.ForDownload)
            {
                var rpt = reportservice.AsXls(reportName, model.Start, model.End, joby);
                return File(rpt, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
            }
            else
            {
                var rpt = reportservice.Get(reportName, model.Start, model.End, joby);
                vm.ReportData = rpt.ReportData;
                vm.Columns = rpt.Columns;
                return View("ReportDetails", vm);
            }
        }


    }
}