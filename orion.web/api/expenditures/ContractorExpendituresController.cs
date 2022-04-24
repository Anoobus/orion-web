using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.api.expenditures.Models;
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
    [Route("orion-api/v1/expenditures/contractors")]
    [ApiController]
    public class ContractorExpendituresController : ControllerBase
    {
        private readonly IClientsRepository _sitesRepository;
        private readonly IMapper _mapper;
        private readonly IInMemRepo<ContractorExpenditure> _repo;
        public ContractorExpendituresController(IClientsRepository clientsRepository, IMapper mapper)
        {
            _repo = new InMemRepo<ContractorExpenditure>();
            _mapper = mapper;
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id:Guid}")]
        public async Task<ActionResult<Page<ContractorExpenditure>>> Get([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name = "week-id")] int weekId,
            [FromRoute(Name = "employee-id")] Guid employeeId)
        {
            var matches = _repo.Search(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToArray();
            return Ok(new Page<ContractorExpenditure>()
            {
                Data = matches,
                Meta = new Meta()
                {
                    Limit = int.MaxValue,
                    Offset = 0,
                    Total = matches.Length
                }
            });
        }


        [HttpPost("week/{week-id:int}/employee/{employee-id:Guid}/job/{job-id:Guid}")]
        public async Task<ActionResult<ContractorExpenditure>> CreateExpenditure([FromBody] EditableContractorExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] Guid employeeId,
           [FromRoute(Name = "job-id")] Guid jobId)
        {
            return Ok(_repo.AddOrUpdate(new ContractorExpenditure()
            {
                Id = Guid.NewGuid(),
                WeekId = weekId,
                EmployeeId = employeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                CompanyName = coolz.CompanyName,
                OrionPONumber = coolz.OrionPONumber,
                TotalPOContractAmount = coolz.TotalPOContractAmount
            }, x => x.Id));

        }


        [HttpPut("{contractor-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableContractorExpenditure coolz,
           [FromRoute(Name = "contractor-expenditure-id")] Guid jobId)
        {
            var match = _repo.FindById(jobId);
            _repo.AddOrUpdate(new ContractorExpenditure()
            {
                Id = Guid.NewGuid(),
                WeekId = match.WeekId,
                EmployeeId = match.EmployeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                CompanyName = coolz.CompanyName,
                OrionPONumber = coolz.OrionPONumber,
                TotalPOContractAmount = coolz.TotalPOContractAmount
            }, x => x.Id);
            return Ok();
        }
    }
}
