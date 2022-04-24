using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures.Models;

namespace orion.web.api.expenditures
{
    public class WeekOfEmployeeExpenditures
    { 
        public IEnumerable<ArcFlashLabelExpenditure> ArcFlashLabelExpenditures { get; set; }
        public IEnumerable<CompanyVehicleExpenditure> CompanyVehicleExpenditures { get; set; }
        public IEnumerable<ContractorExpenditure> ContractorExpenditures { get; set; }
        public IEnumerable<MiscExpenditure> MiscExpenditures { get; set; }
        public IEnumerable<TimeAndExpenceExpenditure> TimeAndExpenceExpenditures { get; set; }
    }

    [Authorize]
    [Route("orion-api/v1/expenditures")]
    [ApiController]
    public class ExpendituresController
    {
        public ExpendituresController()
        {
        }

        [HttpGet("week/{week-id:int}/employee/{employee-id:Guid}")]
        public async Task<ActionResult<WeekOfEmployeeExpenditures>> GetPageOfExpenditure([FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromRoute(Name = "week-id")] int weekId,
            [FromRoute(Name = "employee-id")] int employeeId)
        {
            throw new NotImplementedException(); 
        }
    }
}

