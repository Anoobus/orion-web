using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.Notifications;
using Orion.Web.UI.Models;


namespace Orion.Web.UI.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class SiteController : Controller
    {
        private readonly ISitesRepository siteService;
        private readonly IMapper _mapper;

        public SiteController(ISitesRepository siteService, IMapper mapper)
        {
            this.siteService = siteService;
            _mapper = mapper;
        }

        // GET: Client
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Create()
        {
            return View("Create");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SiteDTO site)
        {
            try
            {
                siteService.Create(site);
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{site.SiteName} has been created.");
                return RedirectToAction(nameof(ListSites));
            }
            catch
            {
                return View();
            }
        }

         public async Task<ActionResult> ListSites()
        {
            var sites = await siteService.GetAll();
            return View("ListSites", new SiteListModel() { Sites = _mapper.Map<IEnumerable<SiteModel>>(sites) });
        }

        public async Task<ActionResult> EditSite(int siteId)
        {
            var site = await siteService.GetSite(siteId);
            if (site == null)
                return NotFound();

            return View("EditSite", _mapper.Map<SiteModel>(site));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSite(SiteModel site)
        {
            await siteService.Save(_mapper.Map<SiteDTO>(site));
            NotificationsController.AddNotification(this.User.SafeUserName(), $"{site.SiteName} was updated");
            return RedirectToAction(nameof(ListSites));
        }
        public async Task<ActionResult> DeleteSite(int siteId)
        {
            try
            {
                await siteService.Delete(siteId);
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Site was deleted");
            } catch(InvalidOperationException ex)
            {
                 NotificationsController.AddNotification(this.User.SafeUserName(), $"Site was not deleted; {ex.Message}");
            }
            
            return RedirectToAction(nameof(ListSites));
        }
    }
}
