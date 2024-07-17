using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Clients;
using Orion.Web.Employees;
using Orion.Web.Notifications;
using Orion.Web.UI.Models;

namespace Orion.Web.UI.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class TestingController : Controller
    {
        public TestingController()
        {
        }

        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
