using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using orion.web.Common;
using orion.web.Jobs;
using orion.web.Reports.Common;

namespace orion.web.BLL.Reports.DetailedExpenseForJobReport
{
    public class DetailedExpenseForJobReportCriteria
    {
         public const string DETAILED_EXPENSE_REPORT_NAME = "Detailed Expense Report";
      
    
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
    }
}

