using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
    public class CompanyVehicleExpenditureOneTimeSet : EditableCompanyVehicleExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableCompanyVehicleExpenditure
    {
        public DateTimeOffset DateVehicleFirstUsed { get; set; }
        [JsonConverter(typeof(StringEnumConverter), false)]
        public CompanyVehicleDescriptor Vehicle { get; set; }
        public int TotalNumberOfDaysUsed { get; set; }
        public int TotalMiles { get; set; }
    }

    public class CompanyVehicleExpenditure : CompanyVehicleExpenditureOneTimeSet
    {

    }


    public enum CompanyVehicleDescriptor
    {
        Unknown = 0,
        Encore,
        Enclave,
        Truck,
    }



    [Authorize]
    [Route("orion-api/v1/expenditures/company-vehicles")]
    [ApiController]
    public class CompanyVehicleExpendituresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInMemRepo<CompanyVehicleExpenditure> _repo;
        public CompanyVehicleExpendituresController( IMapper mapper)
        {
            _mapper = mapper;
            _repo = new InMemRepo<CompanyVehicleExpenditure>();
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id:Guid}")]
        public async Task<ActionResult<Page<CompanyVehicleExpenditure>>> GetPageOfExpenditure([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name = "week-id")] int weekId,
            [FromRoute(Name = "employee-id")] Guid employeeId)
        {
            var matches = _repo.Search(x => x.EmployeeId == employeeId && x.WeekId == weekId).ToArray();
            return Ok(new Page<CompanyVehicleExpenditure>()
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
        public async Task<ActionResult<CompanyVehicleExpenditure>> CreateExpenditure([FromBody] EditableCompanyVehicleExpenditure coolz,
           [FromRoute(Name = "week-id")] int weekId,
           [FromRoute(Name = "employee-id")] Guid employeeId,
           [FromRoute(Name = "job-id")] Guid jobId)
        {
            return Ok(new ArcFlashlabelExpenditure()
            {
                DateOfInvoice = DateTime.Now,
                Id = Guid.NewGuid(),
                WeekId = 1
            });
        }


        [HttpPut("{company-vehicle-expenditure-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableCompanyVehicleExpenditure coolz,
           [FromRoute(Name = "company-vehicle-expenditure-id")] Guid jobId)
        {
            return Ok();
        }
    }
}
