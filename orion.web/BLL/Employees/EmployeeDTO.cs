using System;
using System.Collections.Generic;

namespace orion.web.Employees
{
    public class EmployeeDTO : CoreEmployeeDto
    {
        public Guid ExternalEmployeeId { get; set; }
        public string UserName { get; set; }
        public bool IsExempt { get; set; }
        public List<int> AssignJobs { get; set; }
        public string Role { get; set; }
    }

    public class CoreEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string FullName => $"{Last},{First}";
    }
}
