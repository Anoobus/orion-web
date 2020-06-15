using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using orion.web.Jobs;
using orion.web.Notifications;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository employeeService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IJobService jobService;
        private readonly ICreateEmployeeCommand createEmployeeCommand;
        private readonly IUpdateEmployeeCommand updateEmployeeCommand;

        public EmployeeController(IEmployeeRepository employeeService,
            UserManager<IdentityUser> userManager,
            IJobService jobService,
            ICreateEmployeeCommand createEmployeeCommand,
            IUpdateEmployeeCommand updateEmployeeCommand)
        {
            this.employeeService = employeeService;
            this.userManager = userManager;
            this.jobService = jobService;
            this.createEmployeeCommand = createEmployeeCommand;
            this.updateEmployeeCommand = updateEmployeeCommand;
        }

        public ActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }
        public async Task<ActionResult> List()
        {
            var employees = await employeeService.GetAllEmployees();
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
                IsExempt = emp.IsExempt
            };


            return View("Edit", vm);
        }

        [HttpGet]
        public async Task<ActionResult> NewEmployee()
        {
            var allJobs = await jobService.GetAsync();
            var roles = await employeeService.GetAllRoles();
            var vm = new CreateEmployeeViewModel()
            {
                AvailableJobs = allJobs,
                AvailableRoles = roles
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