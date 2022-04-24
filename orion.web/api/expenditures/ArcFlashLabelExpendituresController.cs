using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures.Models;
using orion.web.BLL;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.Expenditures;
using System;
using System.Threading.Tasks;

namespace orion.web.api
{
    [Authorize]
    [Route("orion-api/v1/expenditures/arc-flash-labels")]
    [ApiController]
    public class ArcFlashLabelExpendituresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMessageHandler<CreateArcFlashLabelExpenditureMessage> _createArcFlashExpenditure;
        private readonly IMessageHandler<UpdateArcFlashLabelExpenditureMessage> _updateArcFlashLabelExpenditure;

        public ArcFlashLabelExpendituresController(IMessageHandler<CreateArcFlashLabelExpenditureMessage> createArcFlashExpenditure,
            IMessageHandler<UpdateArcFlashLabelExpenditureMessage> updateArcFlashLabelExpenditure)
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
            return await _createArcFlashExpenditure.Process(new CreateArcFlashLabelExpenditureMessage(toCreate,weekId,employeeId,jobId));
        }


        [HttpPut("{arc-flash-label-expenditure-id:Guid}")]
        public async Task<IActionResult> UpdateExpenditure([FromBody] EditableArcFlashLabelExpenditure model,
           [FromRoute(Name = "arc-flash-label-expenditure-id")] Guid expId)
        {
            return await _updateArcFlashLabelExpenditure.Process(new UpdateArcFlashLabelExpenditureMessage(model, expId));
        }
    }
}
