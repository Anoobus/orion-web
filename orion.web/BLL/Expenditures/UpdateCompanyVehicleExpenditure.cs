using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.DataAccess;

namespace orion.web.BLL.Expenditures
{
      public class UpdateCompanyVehicleExpenditureMessage : IMessage<CompanyVehicleExpenditure>
    {
        public UpdateCompanyVehicleExpenditureMessage(EditableCompanyVehicleExpenditure model, Guid companyVehicleExpenditureId)
        {
            this.model = model;
            CompanyVehicleExpenditureId = companyVehicleExpenditureId;
        }

        public EditableCompanyVehicleExpenditure model { get; }
        public Guid CompanyVehicleExpenditureId { get; }
    }

    public class UpdateCompanyVehicleExpenditure : HandleBase<UpdateCompanyVehicleExpenditureMessage, CompanyVehicleExpenditure>  
    {
        private readonly ICompanyVehicleExpenditureRepo _companyVehicleExpenditureRepo;
        private readonly IMapper _mapper;

        public UpdateCompanyVehicleExpenditure(ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo, IMapper mapper)
        {
            _companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            _mapper = mapper;
        }

        protected override async Task<IResult> Handle(UpdateCompanyVehicleExpenditureMessage msg)
        {
             var existing = await _companyVehicleExpenditureRepo.FindByExternalId(msg.CompanyVehicleExpenditureId);
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.CompanyVehicleExpenditure>(updateMsg);


            var saved = await _companyVehicleExpenditureRepo.SaveEntity(mapped);
            return _mapper.Map<CompanyVehicleExpenditure>(saved);
        }
    }
}

