using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Jobs;
using Orion.Web.Reports.Common;

namespace Orion.Web.Reports.QuickJobTimeReport
{
    public class QuickJobTimeReportCriteria
    {
        public const string QUICKJOBTIMEREPORTNAME = "Quick Job Time Report";
        public ReportingPeriod PeriodSettings { get; set; }
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
        [Display(Name = "Show All Employees Time For This Job (Not Just Your Time)")]
        public bool ShowAllEmployeesForJob { get; set; }
    }
}
