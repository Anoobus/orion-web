using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures.Models;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.Expenditures;

namespace orion.web.api
{
    [Authorize]
    [Route("orion-api/v1/expenditures/arc-flash-labels")]
    [ApiController]
    public class ArcFlashLabelExpendituresController : ControllerBase
    {
        
        private readonly ICreateArcFlashLabelExpenditure _createArcFlashExpenditure;
        private readonly IUpdateArcFlashLabelExpenditure _updateArcFlashLabelExpenditure;

        public ArcFlashLabelExpendituresController(ICreateArcFlashLabelExpenditure createArcFlashExpenditure,
            IUpdateArcFlashLabelExpenditure updateArcFlashLabelExpenditure)
        {
            _createArcFlashExpenditure = createArcFlashExpenditure;
            _updateArcFlashLabelExpenditure = updateArcFlashLabelExpenditure;
        }

        [HttpPost("week/{week-id:int}/employee/{employee-id}/job/{job-id}")]
        public async Task<IActionResult> CreateExpenditure(
        [FromBody] EditableArcFlashLabelExpenditure toCreate,
        [FromRoute(Name = "week-id")] int weekId,
        [FromRoute(Name = "employee-id")] int employeeId,
        [FromRoute(Name = "job-id")] int jobId)
        {            
            var rez = await _createArcFlashExpenditure.Process(new CreateArcFlashLabelExpenditureMessage(toCreate,weekId,employeeId,jobId));
            return rez.AsApiResult();
        }


        [HttpPut("{arc-flash-label-expenditure-id:Guid}")]
        public async Task<IActionResult> UpdateExpenditure([FromBody] EditableArcFlashLabelExpenditure model,
           [FromRoute(Name = "arc-flash-label-expenditure-id")] Guid expId)
        {
            var rez = await _updateArcFlashLabelExpenditure.Process(new UpdateArcFlashLabelExpenditureMessage(model, expId));
            return rez.AsApiResult();
        }
    }
}
