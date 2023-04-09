using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api;
using orion.web.api.expenditures.Models;
using orion.web.BLL.Core;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.Jobs;
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
        private readonly IJobsRepository _jobsRepository;

        public UpdateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper, IJobsRepository jobsRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<ArcFlashLabelExpenditure>>  Handle(UpdateArcFlashLabelExpenditureMessage msg)
        {
            var job = await _jobsRepository.GetForJobId(msg.model.JobId);
            if (job == null)
                return Failure(ApiErrors.JobDoesNotExistException(msg.model.JobId == 0 ? "[Not Supplied]" : $"[databaseId:{msg.model.JobId }]", "A valid open job must be supplied in order to add an expense to it."));

            if(job.CoreInfo.JobStatusId != JobStatus.Enabled)
                return Failure(ApiErrors.JobMustBeOpen(job.CoreInfo.FullJobCodeWithName, "It must first be enabled/opened in order to add an expense to it. If you do not have access to open the job contact your administrator."));


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
                //NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                //we'll adjust here so we don't drift a day when doing TimeZone Conversion                           
                existing.DateOfInvoice = new DateTimeOffset(msg.model.DateOfInvoice.Year,
                                                                   msg.model.DateOfInvoice.Month,
                                                                   msg.model.DateOfInvoice.Day,
                                                                   0, 0, 0, new TimeSpan());      
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ArcFlashLabelExpenditure>(saved));
        }
    }
}

