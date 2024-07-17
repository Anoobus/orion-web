using System;
using System.Collections.Generic;
using Orion.Web.Employees;
using Orion.Web.Jobs;

namespace Orion.Web.UI.Models
{
    public class ExpenseLandingPageModel
    {
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }
    }
}
