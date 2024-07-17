using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.Reports.Common;

namespace Orion.Web.Reports.EmployeeTimeReport
{
    public class EmployeeTimeReportCriteria
    {
        public const string EMPLOYEETIMEREPORTNAME = "Employee Time Report";
        public ReportingPeriod PeriodSettings { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }
        public int SelectedEmployeeId { get; set; }
    }
}
