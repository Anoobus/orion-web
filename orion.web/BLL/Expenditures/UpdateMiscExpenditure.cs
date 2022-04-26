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
    public class UpdateMiscExpenditureMessage : IProduces<MiscExpenditure>
    {
        public EditableMiscExpenditure Model { get; }
        public Guid ExternalExpenditureId { get; }
        

        public UpdateMiscExpenditureMessage(EditableMiscExpenditure model,
            Guid miscExpId)
        {
            Model = model;
            this.ExternalExpenditureId = miscExpId;
           
        }

        
        
    }

     public interface IUpdateMiscExpenditure
    {
          public Task<IProcessResult<MiscExpenditure>> Process(UpdateMiscExpenditureMessage msg);

    }

    public class UpdateMiscExpenditure
        : Orchastrator<UpdateMiscExpenditureMessage, MiscExpenditure>,
        IUpdateMiscExpenditure, IAutoRegisterAsSingleton
    {
        private readonly IMiscExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public UpdateMiscExpenditure(IMiscExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<MiscExpenditure>>  Handle(UpdateMiscExpenditureMessage msg)
        {
            var existing = await _repo.FindByExternalId(msg.ExternalExpenditureId);
            if(existing == null
                || msg.ExternalExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.MiscExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeOffset.Now;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.ExpensedOn).WeekId.Value;
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.MiscExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<MiscExpenditure>(saved));
        }
    }
}

