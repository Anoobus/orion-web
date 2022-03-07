using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.BLL;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{

    public class ExpenditureOneTimeSet : EditableArcFlashlabelExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableArcFlashlabelExpenditure
    {
        public DateTimeOffset DateOfInvoice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalLabelsCost { get; set; }
        public decimal TotalPostageCost { get; set; }
    }

    public class ArcFlashlabelExpenditure : ExpenditureOneTimeSet, IResult
    {

    }

    [Authorize]
    [Route("orion-api/v1/expenditures/arc-flash-labels")]
    [ApiController]
    public class ArcFlashLabelExpendituresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInMemRepo<DataAccess.EF.ArcFlashlabelExpenditure> _repo;
        public ArcFlashLabelExpendituresController( IMapper mapper)
        {
            _mapper = mapper;
            _repo = new InMemRepo<ArcFlashlabelExpenditure>();
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id}")]
        public async Task<ActionResult<Page<DataAccess.EF.ArcFlashlabelExpenditure>>> GetPageOfExpenditures([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name = "week-id")] int weekId,
            [FromRoute(Name = "employee-id")] int employeeId)
        {
            var matches = _repo.Search(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToArray();
            return Ok(new Page<ArcFlashlabelExpenditure>()
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

        [HttpPost("week/{week-id:int}/employee/{employee-id}/job/{job-id}")]
        public async Task<ActionResult<ArcFlashlabelExpenditure>> CreateExpenditure([FromBody] EditableArcFlashlabelExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] int employeeId,
           [FromRoute(Name = "job-id")] int jobId)
        {
            return Ok(_repo.AddOrUpdate(new ArcFlashlabelExpenditure() {
                DateOfInvoice = coolz.DateOfInvoice,
                Id = Guid.NewGuid(),
                WeekId = weekId,
                EmployeeId = employeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                Quantity = coolz.Quantity,
                TotalLabelsCost = coolz.TotalLabelsCost,
                TotalPostageCost = coolz.TotalPostageCost
            }, x=> x.Id));
        }


        [HttpPut("{arc-flash-label-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableArcFlashlabelExpenditure coolz,
           [FromRoute(Name = "arc-flash-label-expenditure-id")] Guid jobId)
        {
            var match = _repo.FindById(jobId);
            _repo.AddOrUpdate(new ArcFlashlabelExpenditure()
            {
                DateOfInvoice = coolz.DateOfInvoice,
                Id = match.JobId,
                WeekId = match.WeekId,
                EmployeeId = match.EmployeeId,
                JobId = jobId,
                LastModified = DateTimeOffset.Now,
                Quantity = coolz.Quantity,
                TotalLabelsCost = coolz.TotalLabelsCost,
                TotalPostageCost = coolz.TotalPostageCost
            }, x => x.Id);
            return Ok();
        }
    }
}
