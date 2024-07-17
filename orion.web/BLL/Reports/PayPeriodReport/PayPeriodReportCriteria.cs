using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Common;

namespace Orion.Web.Reports.PayPeriodReport
{
    public class PayPeriodReportCriteria
    {
        public const string PAYPERIODREPORTNAME = "Pay Period Report";

        [Display(Name = "Pay Period End Date")]
        public DateTime PayPeriodEnd { get; set; }

        public PayPeriodListDTO PayPeriodList { get; set; }
    }
}
