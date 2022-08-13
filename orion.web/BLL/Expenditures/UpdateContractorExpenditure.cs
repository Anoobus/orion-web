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
    public class UpdateContractorMessage : IProduces<ContractorExpenditure>
    {
        public EditableContractorExpenditure Model { get; }
        public Guid ExternalExpenditureId { get; }
        

        public UpdateContractorMessage(EditableContractorExpenditure model,
            Guid miscExpId)
        {
            Model = model;
            this.ExternalExpenditureId = miscExpId;
           
        }                
    }

     public interface IUpdateContractorExpenditure
    {
          public Task<IProcessResult<ContractorExpenditure>> Process(UpdateContractorMessage msg);

    }

    public class UpdateContractorExpenditure
        : Orchastrator<UpdateContractorMessage, ContractorExpenditure>,
        IUpdateContractorExpenditure, IAutoRegisterAsSingleton
    {
        private readonly IContractorExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public UpdateContractorExpenditure(IContractorExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<ContractorExpenditure>>  Handle(UpdateContractorMessage msg)
        {
            var existing = await _repo.FindByExternalId(msg.ExternalExpenditureId);
            if(existing == null
                || msg.ExternalExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.ContractorExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.ExpensedOn).WeekId.Value;
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ContractorExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ContractorExpenditure>(saved));
        }
    }
}

