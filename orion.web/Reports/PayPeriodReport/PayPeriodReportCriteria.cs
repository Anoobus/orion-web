using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports.PayPeriodReport
{
    public class PayPeriodReportCriteria
    {
        public const string PAY_PERIOD_REPORT_NAME = "Pay Period Report";

        [Display(Name = "Pay Period End Date")]
        public DateTime PayPeriodEnd { get; set; }
    }
}
