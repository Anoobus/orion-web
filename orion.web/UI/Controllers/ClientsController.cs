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
    public class ClientsController : Controller
    {
        private readonly IClientsRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientsController(IClientsRepository clientRepository, IMapper mapper)
        {
            this._clientRepository = clientRepository;
            _mapper = mapper;
        }

        public ActionResult Create()
        {
            return View("CreateClient");
        }

        public async Task<ActionResult> ListClients()
        {
            var clients = await _clientRepository.GetAllClients();
            return View("ListClients", new ClientListModel() { Clients = _mapper.Map<IEnumerable<ClientModel>>(clients) });
        }


        public async Task<ActionResult> EditClient(int clientId)
        {
            var client = await _clientRepository.GetClient(clientId);
            if(client == null)
                return NotFound();

            return View("EditClient", _mapper.Map<ClientModel>(client));
        }

        public async Task<ActionResult> DeleteClient(int clientId)
        {
            await _clientRepository.Delete(clientId);
            return RedirectToAction(nameof(ListClients));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditClient(ClientModel client)
        {
            await _clientRepository.Save(_mapper.Map<ClientDTO>(client));
            NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.ClientName} was updated");
            return RedirectToAction(nameof(ListClients));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientModel client)
        {
            try
            {
                _clientRepository.Save(_mapper.Map< ClientDTO>(client));
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.ClientName} has been created.");
                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View("CreateClient");
            }
        }

    }
}