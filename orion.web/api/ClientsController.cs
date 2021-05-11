using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    [Authorize]
    [Route("api/v1/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsRepository _sitesRepository;
        private readonly IMapper _mapper;

        public ClientsController(IClientsRepository clientsRepository, IMapper mapper)
        {
            _sitesRepository = clientsRepository;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<ClientDTO>>> Get()
        {
            return Ok(await _sitesRepository.GetAllClients());
        }
    }
}
