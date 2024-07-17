using System;
using System.Threading.Tasks;
using AutoMapper;
using Orion.Web.Api;
using Orion.Web.Api.Expenditures.Models;
using Orion.Web.BLL.Core;
using Orion.Web.Common;
using Orion.Web.DataAccess;
using Orion.Web.Jobs;
using Orion.Web.Util.IoC;

namespace Orion.Web.BLL.Expenditures
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
        private readonly IJobsRepository _jobsRepository;

        public UpdateCompanyVehicleExpenditure(ICompanyVehicleExpenditureRepo companyVehicleExpenditureRepo, IMapper mapper, IJobsRepository jobsRepository)
        {
            _companyVehicleExpenditureRepo = companyVehicleExpenditureRepo;
            _mapper = mapper;
            this._jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<CompanyVehicleExpenditure>> Handle(UpdateCompanyVehicleExpenditureMessage msg)
        {
            var job = await _jobsRepository.GetForJobId(msg.Model.JobId);
            if (job == null)
                return Failure(ApiErrors.JobDoesNotExistException(msg.Model.JobId == 0 ? "[Not Supplied]" : $"[databaseId:{msg.Model.JobId}]", "A valid open job must be supplied in order to add an expense to it."));

            if (job.CoreInfo.JobStatusId != JobStatus.Enabled)
                return Failure(ApiErrors.JobMustBeOpen(job.CoreInfo.FullJobCodeWithName, "It must first be enabled/opened in order to add an expense to it. If you do not have access to open the job contact your administrator."));

            var existing = await _companyVehicleExpenditureRepo.FindByExternalId(msg.CompanyVehicleExpenditureId);
            if (existing == null
                || msg.CompanyVehicleExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.CompanyVehicleExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.DateVehicleFirstUsed).WeekId.Value;

                // NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                // we'll adjust here so we don't drift a day when doing TimeZone Conversion
                existing.DateVehicleFirstUsed = new DateTimeOffset(
                    msg.Model.DateVehicleFirstUsed.Year,
                    msg.Model.DateVehicleFirstUsed.Month,
                    msg.Model.DateVehicleFirstUsed.Day,
                    0,
                    0,
                    0,
                    default(TimeSpan));
            }

            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.CompanyVehicleExpenditure>(updateMsg);

            var saved = await _companyVehicleExpenditureRepo.SaveEntity(mapped);
            return Success(_mapper.Map<CompanyVehicleExpenditure>(saved));
        }
    }
}
