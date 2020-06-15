using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;

namespace orion.web.Jobs
{
    [Authorize(Roles =UserRoleName.Admin)]
    public class SiteController : Controller
    {
        private readonly ISitesRepository siteService;

        public SiteController(ISitesRepository siteService)
        {
            this.siteService = siteService;
        }

        // GET: Client
        public ActionResult Index()
        {
            return View();
        }
       

        // GET: Client/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SiteDTO client)
        {
            try
            {
                siteService.Create(client);
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.SiteName} has been created.");
                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View();
            }
        }


    }
}