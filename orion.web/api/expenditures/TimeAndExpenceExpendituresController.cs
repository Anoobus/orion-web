using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using orion.web.api.expenditures;
using orion.web.api.expenditures.Models;

namespace orion.web.api
{
   

    [Authorize]
    [Route("orion-api/v1/expenditures/time-and-expence")]
    [ApiController]
    public class TimeAndExpenceExpendituresController : ControllerBase
    {
        private readonly IClientsRepository _sitesRepository;
        private readonly IMapper _mapper;
        private readonly IInMemRepo<TimeAndExpenceExpenditure> _repo;
        public TimeAndExpenceExpendituresController(IClientsRepository clientsRepository, IMapper mapper)
        {
            _sitesRepository = clientsRepository;
            _mapper = mapper;
            _repo = new InMemRepo<TimeAndExpenceExpenditure>();
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id:Guid}")]
        public async Task<ActionResult<Page<TimeAndExpenceExpenditure>>> Get([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name ="week-id")] int weekId,
            [FromRoute(Name = "employee-id")] int employeeId)
        {
            var matches = _repo.Search(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToArray();
            return Ok(new Page<TimeAndExpenceExpenditure>()
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
        public async Task<ActionResult<TimeAndExpenceExpenditure>> CreateExpenditure([FromBody] EditableTimeAndExpenceExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] int employeeId,
           [FromRoute(Name = "job-id")] int jobId)
        {
            return Ok(_repo.AddOrUpdate(new TimeAndExpenceExpenditure()
            {
                Id = Guid.NewGuid(),
                WeekId = weekId,
                EmployeeId = employeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                 Amount = coolz.Amount
            }, x => x.Id));
        }


        [HttpPut("{time-and-expence-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableTimeAndExpenceExpenditure coolz,
           [FromRoute(Name = "time-and-expence-expenditure-id")] Guid expId)
        {
            var match = _repo.FindById(expId);
            _repo.AddOrUpdate(new TimeAndExpenceExpenditure()
            {
                Id = expId,
                WeekId = match.WeekId,
                EmployeeId = match.EmployeeId,
                JobId = match.JobId,
                LastModified = DateTimeOffset.Now,
                Amount = coolz.Amount,
            }, x => x.Id);
            return Ok();
        }
    }
}
