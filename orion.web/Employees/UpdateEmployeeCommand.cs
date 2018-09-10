using Microsoft.AspNetCore.Identity;
using orion.web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{
    public interface IUpdateEmployeeCommand
    {
        Task<CommandResult> UpdateAsync(EditEmployeeViewModel employee);
    }

    public class UpdateEmployeeCommand : IUpdateEmployeeCommand
    {
        private readonly IEmployeeService employeeService;
        private readonly UserManager<IdentityUser> userManager;

        public UpdateEmployeeCommand(IEmployeeService employeeService, UserManager<IdentityUser> userManager)
        {
            this.employeeService = employeeService;
            this.userManager = userManager;
        }
        public async Task<CommandResult> UpdateAsync(EditEmployeeViewModel employee)
        {
            var allCommandErrors = new List<string>();
            var userRole = employee.SelectedRole;

            var userToUpdate = await userManager.FindByNameAsync(employee.Email);
            var passwordChange = !string.IsNullOrWhiteSpace(employee.Password) &&
            employee.Password.Equals(employee.PasswordConfirm);
            if(passwordChange)
            {
                userToUpdate.PasswordHash = userManager.PasswordHasher.HashPassword(userToUpdate, employee.Password);
                var resulty = await userManager.UpdateAsync(userToUpdate);
                allCommandErrors.AddRange(resulty.Errors.Select(err => $"{err.Code}-{err.Description}"));
            }

            if(!allCommandErrors.Any())
            {
                var existingRoles = await userManager.GetRolesAsync(userToUpdate);
                var hasDifferentRolesThenDesired = existingRoles == null || !existingRoles.Any() || existingRoles.Any(x => x != employee.SelectedRole);
                if(hasDifferentRolesThenDesired)
                {
                    var res1 = await userManager.RemoveFromRolesAsync(userToUpdate, existingRoles);
                    allCommandErrors.AddRange(res1.Errors.Select(err => $"{err.Code}-{err.Description}"));
                    if(res1.Succeeded)
                    {
                        if(employee.SelectedRole == UserRoleName.Disabled)
                        {
                            userToUpdate.LockoutEnabled = true;
                            userToUpdate.LockoutEnd = DateTime.Now.AddYears(100);                            
                            var res2 = await userManager.UpdateAsync(userToUpdate);
                            allCommandErrors.AddRange(res2.Errors.Select(err => $"{err.Code}-{err.Description}"));
                            if(res2.Succeeded)
                            {
                                var existingEmp = employeeService.GetSingleEmployee(employee.Email);
                                existingEmp.Role = employee.SelectedRole;
                                employeeService.Save(existingEmp);
                            }
                        }
                        else
                        {
                            var res2 = await userManager.AddToRoleAsync(userToUpdate, employee.SelectedRole);
                            allCommandErrors.AddRange(res2.Errors.Select(err => $"{err.Code}-{err.Description}"));
                            if(res2.Succeeded)
                            {
                                var existingEmp = employeeService.GetSingleEmployee(employee.Email);
                                existingEmp.Role = employee.SelectedRole;
                                employeeService.Save(existingEmp);
                            }
                        }
                    }
                }
            }

            return new CommandResult(allCommandErrors.Any(), allCommandErrors.ToArray());
        }
    }
}
