using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.DataAccess;

namespace orion.web.BLL.Expenditures
{
    public class UpdateArcFlashLabelExpenditureMessage : IMessage<ArcFlashLabelExpenditure>
    {
        public Guid arcFlashLabelExpenditureId { get; }
        public ArcFlashLabelExpenditureOneTimeSet existingValues {get; set;}

        public UpdateArcFlashLabelExpenditureMessage(EditableArcFlashLabelExpenditure model,
            Guid arcFlashLabelExpenditureId)
        {
            this.model = model;
            this.arcFlashLabelExpenditureId = arcFlashLabelExpenditureId;
           
        }

        public EditableArcFlashLabelExpenditure model { get; }
        
    }

   

    public class UpdateArcFlashLabelExpenditure
        : HandleBase<UpdateArcFlashLabelExpenditureMessage, ArcFlashLabelExpenditure>   
    {
        private readonly IArcFlashLabelExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public UpdateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IResult> Handle(UpdateArcFlashLabelExpenditureMessage msg)
        {
            var existing = await _repo.FindByExternalId(msg.arcFlashLabelExpenditureId);
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return _mapper.Map<ArcFlashLabelExpenditure>(saved);
        }
    }
}

