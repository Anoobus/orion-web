using Microsoft.AspNetCore.Mvc.Rendering;
using orion.web.Jobs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Employees
{
    public class CreateEmployeeViewModel
    {
        public IEnumerable<string> SelectedJobs { get; set; }
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public string SelectedRole { get; set; }
        public IEnumerable<string> AvailableRoles { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }

    public class EditEmployeeViewModel
    {
        public IEnumerable<string> SelectedJobs { get; set; }
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public string SelectedRole { get; set; }
        public IEnumerable<string> AvailableRoles { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }

    public class EmployeeListViewModel
    {
        public EmployeeDTO HeaderHelp { get; set; }
        public List<EmployeeDTO> Employees { get; set; }
    }
   
}
