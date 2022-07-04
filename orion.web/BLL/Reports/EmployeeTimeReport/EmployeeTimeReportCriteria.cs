using orion.web.Employees;
using orion.web.Jobs;
using orion.web.Reports.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Reports.EmployeeTimeReport
{

    public class EmployeeTimeReportCriteria
    {
        public const string EMPLOYEE_TIME_REPORT_NAME = "Employee Time Report";
        public ReportingPeriod PeriodSettings { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }
        public int SelectedEmployeeId { get; set; }
      
    }
}
