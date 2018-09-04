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
        private readonly IEmployeeService employeeService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IJobService jobService;

        public EmployeeController(IEmployeeService employeeService, UserManager<IdentityUser> userManager, IJobService jobService)
        {
            this.employeeService = employeeService;
            this.userManager = userManager;
            this.jobService = jobService;
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
            var allJobs = jobService.Get();
            var roles = await employeeService.GetAllRoles();
            var emp = employeeService.GetSingleEmployee(employee);
            var vm = new EditEmployeeViewModel()
            {
                SelectedRole = emp.Role,
                SelectedJobs = allJobs.Where(x => emp.AssignJobs.Any(z => z == x.JobId)).Select(x => x.JobId.ToString()).ToList(),
                AvailableJobs = allJobs,
                AvailableRoles = roles,
                Email = emp.Name
            };


            return View("Edit", vm);
        }

        [HttpGet]
        public async Task<ActionResult> NewEmployee()
        {
            var allJobs = jobService.Get();
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

            var userRole = employee.SelectedRole;
            var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
            var result = await userManager.CreateAsync(user, employee.Password);
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, userRole);
                employeeService.Save(new EmployeeDTO()
                {
                    Name = employee.Email,
                    AssignJobs = employee.SelectedJobs.Select(x => int.Parse(x)).ToList(),
                    Role = userRole
                });
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{employee.Email} has been created.");
                return RedirectToAction(nameof(NewEmployee));
            }

            return View();

        }


    }
}