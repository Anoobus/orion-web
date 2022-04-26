using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.api.expenditures.Models;
using orion.web.BLL;
using orion.web.BLL.Expenditures;

namespace orion.web.api
{
    [Authorize]
    [Route("orion-api/v1/expenditures/company-vehicles")]
    [ApiController]
    public class CompanyVehicleExpendituresController : ControllerBase
    {
        private readonly IMapper _mapper;        
        private readonly IInMemRepo<CompanyVehicleExpenditure> _repo;
        public CompanyVehicleExpendituresController(IMapper mapper)
        {
            _mapper = mapper;
            
            _repo = new InMemRepo<CompanyVehicleExpenditure>();
        }

        [HttpPost("week/{week-id}/employee/{employee-id}/job/{job-id}")]
        public async Task<IActionResult> CreateExpenditure([FromBody] EditableCompanyVehicleExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] int employeeId,
           [FromRoute(Name = "job-id")] int jobId)
        {
            
               return Ok();
        }


        [HttpPut("{company-vehicle-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableCompanyVehicleExpenditure coolz,
           [FromRoute(Name = "company-vehicle-expenditure-id")] Guid companyVehicleExpenditureId)
        {
            return Ok();
        }
    }
}
