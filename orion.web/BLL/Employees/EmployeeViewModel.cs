using Microsoft.AspNetCore.Mvc.Rendering;
using orion.web.Jobs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Employees
{
    public class CreateEmployeeViewModel
    {
        public IEnumerable<string> SelectedJobs { get; set; }
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        [Required]
        public string SelectedRole { get; set; }
        public IEnumerable<string> AvailableRoles { get; set; }
        [Required]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string PasswordConfirm { get; set; }
        public bool IsExempt { get; set; }
    }

    public class EditEmployeeViewModel
    {
        [Required]
        public string SelectedRole { get; set; }
        public IEnumerable<string> AvailableRoles { get; set; }
        public string NewEmail { get; set; }
        public string Email { get; set; }
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public bool IsExempt { get; set; }
    }

    public class EmployeeListViewModel
    {
        public EmployeeDTO HeaderHelp { get; set; }
        public List<EmployeeDTO> Employees { get; set; }
    }

}
