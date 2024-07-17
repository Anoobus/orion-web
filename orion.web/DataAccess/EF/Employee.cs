using System;
using System.Collections.Generic;

namespace Orion.Web.DataAccess.EF
{
    public class Employee
    {
        public Employee()
        {
            EmployeeJobs = new HashSet<EmployeeJob>();
            EmployeeDirectReports = new HashSet<EmployeeDirectReport>();
        }

        public int EmployeeId { get; set; }
        public Guid ExternalEmployeeId { get; set; }
        public string UserName { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public bool IsExempt { get; set; }
        public ICollection<EmployeeJob> EmployeeJobs { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public ICollection<EmployeeDirectReport> EmployeeDirectReports { get; set; }
    }
}
