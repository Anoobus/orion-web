using System;
using System.Collections.Generic;
using orion.web.Employees;
using orion.web.Jobs;

namespace orion.web.UI.Models
{
    public class ExpenseLandingPageModel
    {                
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }
    }
}

