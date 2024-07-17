using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Employees;
using Serilog;

namespace Orion.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeService;
        private readonly SignInManager<IdentityUser> signInManager;
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext<HomeController>();
        public HomeController(
            IEmployeeRepository employeeService,
            SignInManager<IdentityUser> signInManager)
        {
            this.employeeService = employeeService;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(this.User))
            {
                var emp = await employeeService.GetSingleEmployeeAsync(User.Identity.Name);
                ViewData["first"] = emp.First;
                return View();
            }
            else
            {
                return Redirect(@"/Identity/Account/Login");
            }
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await this.signInManager.SignOutAsync();
            return Redirect(@"/Identity/Account/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                _logger.Error(exceptionFeature.Error, $"ERROR IN MVC CAUGHT for user {this.User?.Identity?.Name}");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
