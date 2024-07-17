using System;
using System.Collections.Generic;
using Orion.Web.Employees;
using Orion.Web.Jobs;

namespace Orion.Web.UI.Models
{
    public class EditExpenseViewModel<T>
    {
        public T CurrentExpenseData { get; set; }

        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public int? NewlySelectedJob { get; set; }

        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }
        public int? NewlySelectedEmployeeId { get; set; }
    }
}
