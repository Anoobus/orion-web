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
    public class UpdateTimeAndExpenseExpenditureMessage : IProduces<TimeAndExpenceExpenditure>
    {
        public EditableTimeAndExpenceExpenditure Model { get; }
        public Guid ExternalExpenditureId { get; }

        public UpdateTimeAndExpenseExpenditureMessage(
            EditableTimeAndExpenceExpenditure model,
            Guid miscExpId)
        {
            Model = model;
            this.ExternalExpenditureId = miscExpId;
        }
    }

    public interface IUpdateTimeAndExpenseExpenditure
    {
        public Task<IProcessResult<TimeAndExpenceExpenditure>> Process(UpdateTimeAndExpenseExpenditureMessage msg);
    }

    public class UpdateTimeAndExpenseExpenditure
        : Orchastrator<UpdateTimeAndExpenseExpenditureMessage, TimeAndExpenceExpenditure>,
        IUpdateTimeAndExpenseExpenditure, IAutoRegisterAsSingleton
    {
        private readonly ITimeAndExpenceExpenditureRepo _repo;
        private readonly IMapper _mapper;
        private readonly IJobsRepository _jobsRepository;

        public UpdateTimeAndExpenseExpenditure(ITimeAndExpenceExpenditureRepo repo, IMapper mapper, IJobsRepository jobsRepository)
        {
            _repo = repo;
            _mapper = mapper;
            this._jobsRepository = jobsRepository;
        }

        protected override async Task<IProcessResult<TimeAndExpenceExpenditure>> Handle(UpdateTimeAndExpenseExpenditureMessage msg)
        {
            var job = await _jobsRepository.GetForJobId(msg.Model.JobId);
            if (job == null)
                return Failure(ApiErrors.JobDoesNotExistException(msg.Model.JobId == 0 ? "[Not Supplied]" : $"[databaseId:{msg.Model.JobId}]", "A valid open job must be supplied in order to add an expense to it."));

            if (job.CoreInfo.JobStatusId != JobStatus.Enabled)
                return Failure(ApiErrors.JobMustBeOpen(job.CoreInfo.FullJobCodeWithName, "It must first be enabled/opened in order to add an expense to it. If you do not have access to open the job contact your administrator."));

            var existing = await _repo.FindByExternalId(msg.ExternalExpenditureId);
            if (existing == null
                || msg.ExternalExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.TimeAndExpenceExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.ExpenseOnDate).WeekId.Value;

                // NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                // we'll adjust here so we don't drift a day when doing TimeZone Conversion
                existing.ExpenseOnDate = new DateTimeOffset(
                    msg.Model.ExpenseOnDate.Year,
                    msg.Model.ExpenseOnDate.Month,
                    msg.Model.ExpenseOnDate.Day,
                    0,
                    0,
                    0,
                    default(TimeSpan));
            }

            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.TimeAndExpenceExpenditure>(updateMsg);

            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<TimeAndExpenceExpenditure>(saved));
        }
    }
}
