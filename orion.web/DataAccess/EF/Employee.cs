using System.Collections.Generic;

namespace orion.web.DataAccess.EF
{
    public class Employee
    {
        public Employee()
        {
            EmployeeJobs = new HashSet<EmployeeJob>();
        }
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public bool IsExempt { get; set; }
        public ICollection<EmployeeJob> EmployeeJobs { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
