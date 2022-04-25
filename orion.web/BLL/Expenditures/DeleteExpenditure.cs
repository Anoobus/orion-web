using System;
using System.Threading.Tasks;
using orion.web.api;
using orion.web.BLL.Core;
using orion.web.DataAccess;
using orion.web.UI.Models;
using orion.web.Util.IoC;

namespace orion.web.BLL.Expenditures
{
    public class DeleteExpenditureRequest : IProduces<EmptyResult>
    {
        public DeleteExpenditureRequest(Guid expenditureToDelete)
        {
            ExpenditureToDelete = expenditureToDelete;
        }

        public Guid ExpenditureToDelete { get; }
    }

     public interface IDeleteExpenditure
    {
          public Task<IProcessResult<EmptyResult>> Process(DeleteExpenditureRequest msg);
    }
    public class DeleteExpenditure
        : Orchastrator<DeleteExpenditureRequest, EmptyResult>,
        IDeleteExpenditure, IAutoRegisterAsSingleton

    {
        private readonly IArcFlashLabelExpenditureRepo arcFlashLabelExpenditureRepo;
        private readonly ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo;
        private readonly IContractorExpenditureRepo contractorExpenditureRepo;
        private readonly ITimeAndExpenceExpenditureRepo timeAndExpenceExpenditureRepo;
        private readonly IMiscExpenditureRepo miscExpenditureRepo;
        private readonly ICoreExpenditureRepo coreExpenditureRepo;

        public DeleteExpenditure(IArcFlashLabelExpenditureRepo arcFlashLabelExpenditureRepo,
            ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo,
            IContractorExpenditureRepo contractorExpenditureRepo,
            ITimeAndExpenceExpenditureRepo timeAndExpenceExpenditureRepo,
            IMiscExpenditureRepo miscExpenditureRepo,
            ICoreExpenditureRepo coreExpenditureRepo)
        {
            this.arcFlashLabelExpenditureRepo = arcFlashLabelExpenditureRepo;
            this.companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            this.contractorExpenditureRepo = contractorExpenditureRepo;
            this.timeAndExpenceExpenditureRepo = timeAndExpenceExpenditureRepo;
            this.miscExpenditureRepo = miscExpenditureRepo;
            this.coreExpenditureRepo = coreExpenditureRepo;
        }
        protected override async Task<IProcessResult<EmptyResult>> Handle(DeleteExpenditureRequest msg)
        {
            var exp = await coreExpenditureRepo.GetExpenditureCoreInfoByExternalId(msg.ExpenditureToDelete);
            if (exp == null)
                return Failure(ApiErrors.NotFoundException($"No expense exists with id: {msg.ExpenditureToDelete}"));

                    
            switch (exp.ExpenditureType)
            {
                case ExpenditureTypeEnum.ArcFlashLabelExpenditure:
                    await arcFlashLabelExpenditureRepo.DeleteEntity(msg.ExpenditureToDelete);
                    break;
                case ExpenditureTypeEnum.CompanyVehicleExpenditure:
                    await companyVehicleExpenditureRepo.DeleteEntity(msg.ExpenditureToDelete);
                    break;
                case ExpenditureTypeEnum.ContractorExpenditure:
                    await contractorExpenditureRepo.DeleteEntity(msg.ExpenditureToDelete);
                    break;
                case ExpenditureTypeEnum.MiscExpenditure:
                    await miscExpenditureRepo.DeleteEntity(msg.ExpenditureToDelete);
                    break;
                case ExpenditureTypeEnum.TimeAndExpenceExpenditure:
                    await timeAndExpenceExpenditureRepo.DeleteEntity(msg.ExpenditureToDelete);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return Success(EmptyResult.Instance);
        }
    }

}

