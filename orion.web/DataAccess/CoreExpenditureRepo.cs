using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Orion.Web.BLL.Expenditures;
using Orion.Web.Util.IoC;

namespace Orion.Web.DataAccess
{
    public interface ICoreExpenditureRepo
    {
        Task<CoreExpenditureDto> GetExpenditureCoreInfoByExternalId(Guid ExternalId);
    }

    public class CoreExpenditureRepo : ICoreExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IArcFlashLabelExpenditureRepo arcFlashLabelExpenditureRepo;
        private readonly ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo;
        private readonly IContractorExpenditureRepo contractorExpenditureRepo;
        private readonly ITimeAndExpenceExpenditureRepo timeAndExpenceExpenditureRepo;
        private readonly IMiscExpenditureRepo miscExpenditureRepo;
        private static readonly ConcurrentDictionary<Guid, CoreExpenditureDto> _cachedInfo = new ConcurrentDictionary<Guid, CoreExpenditureDto>();
        public CoreExpenditureRepo(
            IArcFlashLabelExpenditureRepo arcFlashLabelExpenditureRepo,
            ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo,
            IContractorExpenditureRepo contractorExpenditureRepo,
            ITimeAndExpenceExpenditureRepo timeAndExpenceExpenditureRepo,
            IMiscExpenditureRepo miscExpenditureRepo)
        {
            this.arcFlashLabelExpenditureRepo = arcFlashLabelExpenditureRepo;
            this.companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            this.contractorExpenditureRepo = contractorExpenditureRepo;
            this.timeAndExpenceExpenditureRepo = timeAndExpenceExpenditureRepo;
            this.miscExpenditureRepo = miscExpenditureRepo;
        }

        public async Task<CoreExpenditureDto> GetExpenditureCoreInfoByExternalId(Guid ExternalId)
        {
            if (_cachedInfo.TryGetValue(ExternalId, out var expenditure))
                return expenditure;

            expenditure = await SearchForExpenditure(ExternalId);

            if (expenditure != null)
                _cachedInfo.TryAdd(ExternalId, expenditure);

            return expenditure;
        }

        private async Task<CoreExpenditureDto> SearchForExpenditure(Guid externalId)
        {
            var afl = await arcFlashLabelExpenditureRepo.FindByExternalId(externalId);
            if (afl != null)
            {
                return new CoreExpenditureDto()
                {
                    ExpenditureType = ExpenditureTypeEnum.ArcFlashLabelExpenditure,
                    ExternalId = externalId
                };
            }

            var cve = await companyVehicleExpenditureRepo.FindByExternalId(externalId);
            if (cve != null)
            {
                return new CoreExpenditureDto()
                {
                    ExpenditureType = ExpenditureTypeEnum.CompanyVehicleExpenditure,
                    ExternalId = externalId
                };
            }

            var ce = await contractorExpenditureRepo.FindByExternalId(externalId);
            if (ce != null)
            {
                return new CoreExpenditureDto()
                {
                    ExpenditureType = ExpenditureTypeEnum.ContractorExpenditure,
                    ExternalId = externalId
                };
            }

            var me = await miscExpenditureRepo.FindByExternalId(externalId);
            if (me != null)
            {
                return new CoreExpenditureDto()
                {
                    ExpenditureType = ExpenditureTypeEnum.MiscExpenditure,
                    ExternalId = externalId
                };
            }

            var taee = await timeAndExpenceExpenditureRepo.FindByExternalId(externalId);
            if (taee != null)
            {
                return new CoreExpenditureDto()
                {
                    ExpenditureType = ExpenditureTypeEnum.TimeAndExpenceExpenditure,
                    ExternalId = externalId
                };
            }

            return null;
        }
    }
}
