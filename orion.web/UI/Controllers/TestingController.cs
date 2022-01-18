using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace orion.web.UI.Controllers
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