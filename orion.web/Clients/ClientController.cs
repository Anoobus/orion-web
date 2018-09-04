using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;

namespace orion.web.Clients
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class ClientController : Controller
    {
        private readonly IClientService clientService;

        public ClientController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientDTO client)
        {
            try
            {
                clientService.Post(client);
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.FullName} has been created.");                
                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View();
            }
        }

    }
}