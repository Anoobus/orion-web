using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;

namespace orion.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeService;

        public HomeController(IEmployeeRepository employeeService)
        {
            this.employeeService = employeeService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var emp =await  employeeService.GetSingleEmployeeAsync(User.Identity.Name);
            ViewData["first"] = emp.First;
            return View();
        }

        [Authorize(Roles =UserRoleName.Admin)]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
