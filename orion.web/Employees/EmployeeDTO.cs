using System.Collections.Generic;

namespace orion.web.Employees
{ 
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<int> AssignJobs { get; set; }
        public string Role { get; set; }
    }
}
