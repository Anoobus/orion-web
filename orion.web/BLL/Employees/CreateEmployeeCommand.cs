using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Orion.Web.Common;
using Orion.Web.Util.IoC;

namespace Orion.Web.Employees
{
    public interface ICreateEmployeeCommand
    {
        Task<Result> Create(CreateEmployeeViewModel employee);
    }

    public class CreateEmployeeCommand : ICreateEmployeeCommand, IAutoRegisterAsTransient
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmployeeRepository _employeeService;

        public CreateEmployeeCommand(UserManager<IdentityUser> userManager, IEmployeeRepository employeeService)
        {
            _userManager = userManager;
            _employeeService = employeeService;
        }

        public async Task<Result> Create(CreateEmployeeViewModel employee)
        {
            var userRole = employee.SelectedRole;
            var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
            var passwordsMatch = employee.Password.Equals(employee.PasswordConfirm);
            if (!passwordsMatch)
            {
                return new Result(false, "Passwords don't match");
            }

            var isNewEmployee = (await _userManager.FindByNameAsync(employee.Email)) == null;
            if (!passwordsMatch)
            {
                return new Result(false, $"{employee.Email} has already been created.");
            }

            // only supervisors can persist direct reports
            if (employee.SelectedRole != UserRoleName.Supervisor)
                employee.SelectedDirectReports = new int[0];

            var allEmployees = (await _employeeService.GetAllEmployees()).Select(x => x.EmployeeId)
                                                                         .ToHashSet();

            var result = await _userManager.CreateAsync(user, employee.Password);
            if (result.Succeeded)
            {
                var employeesJobs = employee.SelectedJobs == null ? new List<int>() : employee.SelectedJobs.Select(x => int.Parse(x)).ToList();
                await _userManager.AddToRoleAsync(user, userRole);

                _employeeService.Save(new EmployeeDTO()
                {
                    UserName = employee.Email,
                    AssignJobs = employeesJobs,
                    Role = userRole,
                    First = employee.FirstName,
                    Last = employee.LastName,
                    IsExempt = employee.IsExempt,
                    DirectReports = employee.SelectedDirectReports.Where(x => allEmployees.Contains(x))
                                                                  .ToList()
                });

                return new Result(true);
            }
            else
            {
                return new Result(false, result.Errors.Select(err => $"{err.Code}-{err.Description}").ToArray());
            }
        }
    }
}
