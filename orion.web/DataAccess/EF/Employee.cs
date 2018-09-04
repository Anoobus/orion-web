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
        public string Name { get; set; }
        public ICollection<EmployeeJob> EmployeeJobs { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
