using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;

namespace orion.web.Clients
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class ClientController : Controller
    {
        private readonly IClientsRepository clientService;

        public ClientController(IClientsRepository clientService)
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
                clientService.Create(client);
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