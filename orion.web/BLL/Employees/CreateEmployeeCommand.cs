﻿using Microsoft.AspNetCore.Identity;
using orion.web.Common;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{
    public interface ICreateEmployeeCommand
    {
        Task<Result> Create(CreateEmployeeViewModel employee);
    }
    public class CreateEmployeeCommand : ICreateEmployeeCommand, IAutoRegisterAsTransient
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmployeeRepository employeeService;

        public CreateEmployeeCommand(UserManager<IdentityUser> userManager, IEmployeeRepository employeeService)
        {
            this.userManager = userManager;
            this.employeeService = employeeService;
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
            var isNewEmployee = (await userManager.FindByNameAsync(employee.Email)) == null;
            if (!passwordsMatch)
            {
                return new Result(false, $"{employee.Email} has already been created.");
            }

            var result = await userManager.CreateAsync(user, employee.Password);
            if (result.Succeeded)
            {
                var employeesJobs = employee.SelectedJobs == null ? new List<int>() : employee.SelectedJobs.Select(x => int.Parse(x)).ToList();
                await userManager.AddToRoleAsync(user, userRole);
                employeeService.Save(new EmployeeDTO()
                {
                    UserName = employee.Email,
                    AssignJobs = employeesJobs,
                    Role = userRole,
                    First = employee.FirstName,
                    Last = employee.LastName,
                    IsExempt = employee.IsExempt,
                });
                return new Result(true);
            }
            else
            {
                return new Result(false, result.Errors.Select(err => $"{ err.Code}-{err.Description}").ToArray());
            }

        }
    }
}
