using System;
using System.Collections.Generic;

namespace Orion.Web.Employees
{
    public class EmployeeDTO : CoreEmployeeDto
    {
        public Guid ExternalEmployeeId { get; set; }
        public string UserName { get; set; }
        public bool IsExempt { get; set; }
        public List<int> AssignJobs { get; set; }
        public List<int> DirectReports { get; set; }
        public string Role { get; set; }
        public string NameOfRecord => $"{Last}, {First}";
    }

    public class CoreEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string FullName => $"{Last},{First}";
    }
}
