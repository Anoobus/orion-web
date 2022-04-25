using System;
using System.Collections.Generic;
using orion.web.Employees;
using orion.web.Jobs;

namespace orion.web.UI.Models
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

