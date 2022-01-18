using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    public class MiscExpenditureOneTimeSet : EditableMiscExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableMiscExpenditure
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class MiscExpenditure : MiscExpenditureOneTimeSet
    {

    }

    [Authorize]
    [Route("orion-api/v1/expenditures/misc")]
    [ApiController]
    public class MiscExpendituresController : ControllerBase
    {
        private readonly IClientsRepository _sitesRepository;
        private readonly IMapper _mapper;
        private readonly IInMemRepo<MiscExpenditure> _repo;
        public MiscExpendituresController(IClientsRepository clientsRepository, IMapper mapper)
        {
            _repo = new InMemRepo<MiscExpenditure>();
            _mapper = mapper;
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id:Guid}")]
        public async Task<ActionResult<Page<MiscExpenditure>>> Get([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name = "week-id")] int weekId,
            [FromRoute(Name = "employee-id")] Guid employeeId)
        {
            var matches = _repo.Search(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToArray();
            return Ok(new Page<MiscExpenditure>()
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
        public async Task<ActionResult<MiscExpenditure>> CreateExpenditure([FromBody] EditableMiscExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] Guid employeeId,
           [FromRoute(Name = "job-id")] Guid jobId)
        {
            return Ok(_repo.AddOrUpdate(new MiscExpenditure()
            {
                Id = Guid.NewGuid(),
                WeekId = weekId,
                EmployeeId = employeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                 Amount = coolz.Amount,
                  Description = coolz.Description
            }, x => x.Id));
        }


        [HttpPut("{mis-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableMiscExpenditure coolz,
           [FromRoute(Name = "mis-expenditure-id")] Guid jobId)
        {
            var match = _repo.FindById(jobId);
            _repo.AddOrUpdate(new MiscExpenditure()
            {
                Id = match.JobId,
                WeekId = match.WeekId,
                EmployeeId = match.EmployeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                Amount = coolz.Amount,
                Description = coolz.Description
            }, x => x.Id);
            return Ok();
        }
    }
}
