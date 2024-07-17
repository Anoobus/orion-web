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
    public class UpdateArcFlashLabelExpenditureMessage : IProduces<ArcFlashLabelExpenditure>
    {
        public Guid ArcFlashLabelExpenditureId { get; }
        public ArcFlashLabelExpenditure ExistingValues { get; set; }

        public UpdateArcFlashLabelExpenditureMessage(
            EditableArcFlashLabelExpenditure model,
            Guid arcFlashLabelExpenditureId)
        {
            this.Model = model;
            this.ArcFlashLabelExpenditureId = arcFlashLabelExpenditureId;
        }

        public EditableArcFlashLabelExpenditure Model { get; }
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
        private readonly IJobsRepository _jobsRepository;

        public UpdateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper, IJobsRepository jobsRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<ArcFlashLabelExpenditure>> Handle(UpdateArcFlashLabelExpenditureMessage msg)
        {
            var job = await _jobsRepository.GetForJobId(msg.Model.JobId);
            if (job == null)
                return Failure(ApiErrors.JobDoesNotExistException(msg.Model.JobId == 0 ? "[Not Supplied]" : $"[databaseId:{msg.Model.JobId}]", "A valid open job must be supplied in order to add an expense to it."));

            if (job.CoreInfo.JobStatusId != JobStatus.Enabled)
                return Failure(ApiErrors.JobMustBeOpen(job.CoreInfo.FullJobCodeWithName, "It must first be enabled/opened in order to add an expense to it. If you do not have access to open the job contact your administrator."));

            var existing = await _repo.FindByExternalId(msg.ArcFlashLabelExpenditureId);
            if (existing == null
                || msg.ArcFlashLabelExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.ArcFlashLabelExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.DateOfInvoice.DateTime).WeekId.Value;
                var tester = WeekDTO.CreateWithWeekContaining(msg.Model.DateOfInvoice).WeekId.Value;

                // NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                // we'll adjust here so we don't drift a day when doing TimeZone Conversion
                existing.DateOfInvoice = new DateTimeOffset(
                    msg.Model.DateOfInvoice.Year,
                    msg.Model.DateOfInvoice.Month,
                    msg.Model.DateOfInvoice.Day,
                    0,
                    0,
                    0,
                    default(TimeSpan));
            }

            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(updateMsg);

            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ArcFlashLabelExpenditure>(saved));
        }
    }
}
