using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orion.Web.Common;
using Orion.Web.Jobs;
using Orion.Web.Reports.Common;

namespace Orion.Web.BLL.Reports.DetailedExpenseForJobReport
{
    public class DetailedExpenseForJobReportCriteria
    {
        public const string DETAILEDEXPENSEREPORTNAME = "Detailed Expense Report";

        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }
}
