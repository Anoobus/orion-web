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
    public class UpdateCompanyVehicleExpenditureMessage : IProduces<CompanyVehicleExpenditure>
    {
        public EditableCompanyVehicleExpenditure Model { get; }
        public Guid CompanyVehicleExpenditureId { get; }
        public UpdateCompanyVehicleExpenditureMessage(EditableCompanyVehicleExpenditure model, Guid companyVehicleExpenditureId)
        {
            Model = model;
            CompanyVehicleExpenditureId = companyVehicleExpenditureId;
        }        
    }

    public interface IUpdateCompanyVehicleExpenditure
    {
          public Task<IProcessResult<CompanyVehicleExpenditure>> Process(UpdateCompanyVehicleExpenditureMessage msg);

    }

    public class UpdateCompanyVehicleExpenditure
        : Orchastrator<UpdateCompanyVehicleExpenditureMessage, CompanyVehicleExpenditure>,
        IUpdateCompanyVehicleExpenditure, IAutoRegisterAsSingleton
    {
        private readonly ICompanyVehicleExpenditureRepo _companyVehicleExpenditureRepo;
        private readonly IMapper _mapper;

        public UpdateCompanyVehicleExpenditure(ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo, IMapper mapper)
        {
            _companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<CompanyVehicleExpenditure>> Handle(UpdateCompanyVehicleExpenditureMessage msg)
        {
            var existing = await _companyVehicleExpenditureRepo.FindByExternalId(msg.CompanyVehicleExpenditureId);
            if(existing == null
                || msg.CompanyVehicleExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.CompanyVehicleExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.DateVehicleFirstUsed).WeekId.Value;
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.CompanyVehicleExpenditure>(updateMsg);
            
            var saved = await _companyVehicleExpenditureRepo.SaveEntity(mapped);
            return Success(_mapper.Map<CompanyVehicleExpenditure>(saved));
        }
    }
}

