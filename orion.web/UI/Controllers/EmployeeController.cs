using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Common;
using Orion.Web.Jobs;
using Orion.Web.Notifications;

namespace Orion.Web.Employees
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository employeeService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IJobsRepository jobService;
        private readonly ICreateEmployeeCommand createEmployeeCommand;
        private readonly IUpdateEmployeeCommand updateEmployeeCommand;
        private readonly ISessionAdapter _sessionAdapter;

        public EmployeeController(
            IEmployeeRepository employeeService,
            UserManager<IdentityUser> userManager,
            IJobsRepository jobService,
            ICreateEmployeeCommand createEmployeeCommand,
            IUpdateEmployeeCommand updateEmployeeCommand,
            ISessionAdapter sessionAdapter)
        {
            this.employeeService = employeeService;
            this.userManager = userManager;
            this.jobService = jobService;
            this.createEmployeeCommand = createEmployeeCommand;
            this.updateEmployeeCommand = updateEmployeeCommand;
            _sessionAdapter = sessionAdapter;
        }

        public ActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        public async Task<ActionResult> List()
        {
            var employees = await employeeService.GetAllEmployees();
            employees = employees.Where(x => x.UserName != "admin@company.com").ToList();
            var vm = new EmployeeListViewModel()
            {
                Employees = employees.ToList()
            };
            return View("List", vm);
        }

        [HttpGet("[Controller]/Edit/{employee}")]
        public async Task<ActionResult> Edit(string employee)
        {
            var allJobs = jobService.GetAsync();
            var roles = await employeeService.GetAllRoles();
            var emp = await employeeService.GetSingleEmployeeAsync(employee);
            
            var vm = new EditEmployeeViewModel()
            {
                SelectedRole = emp.Role,
                AvailableRoles = roles,
                Email = emp.UserName,
                FirstName = emp.First,
                LastName = emp.Last,
                IsExempt = emp.IsExempt,
                NewEmail = string.Empty,
                Password = string.Empty,
                PasswordConfirm = string.Empty,
                SelectedDirectReports = emp.DirectReports,
                AvailableDirectReports = await GetAvailableDirectReportEmployees(emp.EmployeeId)
            };

            return View("Edit", vm);
        }

        private async Task<EmployeeSelectionViewModel[]> GetAvailableDirectReportEmployees(int? supervisorsEmployeeId = null)
        {
            var allEmps = await employeeService.GetAllEmployees();
            return allEmps.Where(x => x.UserName != "admin@company.com" 
                                      && x.Role != UserRoleName.Disabled 
                                      && (!supervisorsEmployeeId.HasValue || x.EmployeeId != supervisorsEmployeeId.Value) 
                                      && x.Role != UserRoleName.Admin)
                          .Select(x => new EmployeeSelectionViewModel()
                          {
                              EmployeeId = x.EmployeeId,
                              Fullname = x.NameOfRecord
                          })
                          .OrderBy(x => x.Fullname)
                          .ToArray();
        }

        [HttpGet]
        public async Task<ActionResult> NewEmployee()
        {
            var allJobs = await jobService.GetAsync();
            var roles = await employeeService.GetAllRoles();
           
            var vm = new CreateEmployeeViewModel()
            {
                AvailableJobs = allJobs,
                AvailableRoles = roles,
                AvailableDirectReports = await GetAvailableDirectReportEmployees()
            };
            return View("NewEmployee", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewEmployee(CreateEmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                var res = await createEmployeeCommand.Create(employee);
                if (res.Successful)
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), $"{employee.Email} has been created.");
                    return RedirectToAction(nameof(NewEmployee));
                }
                else
                {
                    foreach (var err in res.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                }
            }

            employee.AvailableJobs = await jobService.GetAsync();
            employee.AvailableRoles = await employeeService.GetAllRoles();
            return View("NewEmployee", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEmployee(EditEmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                var res = await updateEmployeeCommand.UpdateAsync(employee);

                if (res.Successful)
                {
                    NotificationsController.AddNotification(this.User.SafeUserName(), $"{employee.Email} has been updated.");
                    return RedirectToAction(nameof(Edit), new { employee = employee.Email });
                }
                else
                {
                    foreach (var err in res.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                }
            }

            employee.AvailableRoles = await employeeService.GetAllRoles();
            return View("Edit", employee);
        }
    }
}
