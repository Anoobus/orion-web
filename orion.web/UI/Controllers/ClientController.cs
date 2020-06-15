using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;

namespace orion.web.Clients
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class ClientController : Controller
    {
        private readonly IClientsRepository _clientRepository;

        public ClientController(IClientsRepository clientRepository)
        {
            this._clientRepository = clientRepository;
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
                _clientRepository.Create(client);
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.ClientName} has been created.");
                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View();
            }
        }

    }
}