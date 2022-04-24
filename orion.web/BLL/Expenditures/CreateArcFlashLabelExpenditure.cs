using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.DataAccess;

namespace orion.web.BLL.ArcFlashExpenditureExpenses
{
    public interface IMessageProcessor
    {
        Task<IActionResult> Process<T>(T msg);
    }
    public class MessageProcessor
    {
        private readonly IServiceProvider _sp;

        public MessageProcessor(IServiceProvider sp)
        {
            _sp = sp;
        }
        public Task<IActionResult> Process<T>(T msg)
        {
            return _sp.GetRequiredService<IMessageHandler<T>>().Process(msg);
        }
    }

    public class CreateArcFlashLabelExpenditureMessage : IMessage<ArcFlashLabelExpenditure>
    {
        public CreateArcFlashLabelExpenditureMessage(EditableArcFlashLabelExpenditure model, int weekId, int employeeId, int jobId)
        {
            this.model = model;
            this.weekId = weekId;
            this.employeeId = employeeId;
            this.jobId = jobId;
        }

        public EditableArcFlashLabelExpenditure model { get; }
        public int weekId { get; }
        public int employeeId { get; }
        public int jobId { get; }
    }

   

    public class CreateArcFlashLabelExpenditure
        : HandleBase<CreateArcFlashLabelExpenditureMessage, ArcFlashLabelExpenditure>   
    {
        private readonly IArcFlashLabelExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public CreateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IResult> Handle(CreateArcFlashLabelExpenditureMessage msg)
        {
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(msg);
            var saved = await _repo.SaveEntity(mapped);
            return _mapper.Map<ArcFlashLabelExpenditure>(saved);
        }
    }
}

