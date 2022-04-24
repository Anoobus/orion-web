using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api.expenditures.Models;
using orion.web.DataAccess;

namespace orion.web.BLL.Expenditures
{
      public class CreateCompanyVehicleExpenditureMessage : IMessage<CompanyVehicleExpenditure>
    {
        public CreateCompanyVehicleExpenditureMessage(EditableCompanyVehicleExpenditure model, int weekId, int employeeId, int jobId)
        {
            this.model = model;
            this.weekId = weekId;
            this.employeeId = employeeId;
            this.jobId = jobId;
        }

        public EditableCompanyVehicleExpenditure model { get; }
        public int weekId { get; }
        public int employeeId { get; }
        public int jobId { get; }
    }

    public class CreateCompanyVehicleExpenditure : HandleBase<CreateCompanyVehicleExpenditureMessage, CompanyVehicleExpenditure>  
    {
        private readonly ICompanyVehicleExpenditureRepo _companyVehicleExpenditureRepo;
        private readonly IMapper _mapper;

        public CreateCompanyVehicleExpenditure(ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo, IMapper mapper)
        {
            _companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            _mapper = mapper;
        }

        protected override async Task<IResult> Handle(CreateCompanyVehicleExpenditureMessage msg)
        {
             var mapped = _mapper.Map<DataAccess.EF.CompanyVehicleExpenditure>(msg);
            var saved = await _companyVehicleExpenditureRepo.SaveEntity(mapped);
            return _mapper.Map<CompanyVehicleExpenditure>(saved);
        }
    }
}

