using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.UI.api
{
    [Authorize]
    [Route("api/v1/sites")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly ISitesRepository _sitesRepository;
        private readonly IMapper _mapper;

        public SiteController(ISitesRepository sitesRepository, IMapper mapper)
        {
            _sitesRepository = sitesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteModel>>> Get()
        {
            return Ok(_mapper.Map<IEnumerable<SiteModel>>( await _sitesRepository.GetAll()));
        }
    }
}
