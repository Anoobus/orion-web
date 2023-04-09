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
        private readonly IJobsRepository _jobsRepository;

        public UpdateContractorExpenditure(IContractorExpenditureRepo repo, IMapper mapper, IJobsRepository jobsRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<ContractorExpenditure>>  Handle(UpdateContractorMessage msg)
        {
            var job = await _jobsRepository.GetForJobId(msg.Model.JobId);
            if (job == null)
                return Failure(ApiErrors.JobDoesNotExistException(msg.Model.JobId == 0 ? "[Not Supplied]" : $"[databaseId:{msg.Model.JobId }]", "A valid open job must be supplied in order to add an expense to it."));

            if(job.CoreInfo.JobStatusId != JobStatus.Enabled)
                return Failure(ApiErrors.JobMustBeOpen(job.CoreInfo.FullJobCodeWithName, "It must first be enabled/opened in order to add an expense to it. If you do not have access to open the job contact your administrator."));

            var existing = await _repo.FindByExternalId(msg.ExternalExpenditureId);
            if(existing == null
                || msg.ExternalExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.ContractorExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.ExpensedOn).WeekId.Value;
                //NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                //we'll adjust here so we don't drift a day when doing TimeZone Conversion
                existing.ExpensedOn = new DateTimeOffset(msg.Model.ExpensedOn.Year,
                                                                   msg.Model.ExpensedOn.Month,
                                                                   msg.Model.ExpensedOn.Day,
                                                                   0, 0, 0, new TimeSpan());     
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.ContractorExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ContractorExpenditure>(saved));
        }
    }
}

