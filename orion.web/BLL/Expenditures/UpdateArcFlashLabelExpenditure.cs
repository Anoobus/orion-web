using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.BLL.Core;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.Util.IoC;

namespace orion.web.BLL.Expenditures
{
    public class UpdateArcFlashLabelExpenditureMessage : IProduces<ArcFlashLabelExpenditure>
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

     public interface IUpdateArcFlashLabelExpenditure
    {
          public Task<IProcessResult<ArcFlashLabelExpenditure>> Process(UpdateArcFlashLabelExpenditureMessage msg);

    }

    public class UpdateArcFlashLabelExpenditure
        : Orchastrator<UpdateArcFlashLabelExpenditureMessage, ArcFlashLabelExpenditure>,
        IUpdateArcFlashLabelExpenditure, IAutoRegisterAsSingleton
    {
        private readonly IArcFlashLabelExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public UpdateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<ArcFlashLabelExpenditure>>  Handle(UpdateArcFlashLabelExpenditureMessage msg)
        {
            var existing = await _repo.FindByExternalId(msg.arcFlashLabelExpenditureId);
            if(existing == null
                || msg.arcFlashLabelExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.ArcFlashLabelExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.model.DateOfInvoice.DateTime).WeekId.Value;
                var tester = WeekDTO.CreateWithWeekContaining(msg.model.DateOfInvoice).WeekId.Value;
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ArcFlashLabelExpenditure>(saved));
        }
    }
}

