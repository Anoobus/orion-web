using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports.Common
{
    public class ReportingPeriod
    {
        [Display(Name = "Report Start Date")]
        public DateTime Start { get; set; }
        [Display(Name = "Report End Date")]
        public DateTime End { get; set; }
    }
}
