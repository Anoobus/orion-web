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
    public class UpdateTimeAndExpenseExpenditureMessage : IProduces<TimeAndExpenceExpenditure>
    {
        public EditableTimeAndExpenceExpenditure Model { get; }
        public Guid ExternalExpenditureId { get; }
        

        public UpdateTimeAndExpenseExpenditureMessage(EditableTimeAndExpenceExpenditure model,
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

        public UpdateTimeAndExpenseExpenditure(ITimeAndExpenceExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<TimeAndExpenceExpenditure>>  Handle(UpdateTimeAndExpenseExpenditureMessage msg)
        {
            var existing = await _repo.FindByExternalId(msg.ExternalExpenditureId);
            if(existing == null
                || msg.ExternalExpenditureId == default(Guid))
            {
                existing = new DataAccess.EF.TimeAndExpenceExpenditure();
                existing.ExternalId = Guid.NewGuid();
                existing.Id = 0;
                existing.LastModified = DateTimeWithZone.EasternStandardTimeOffset;
                existing.WeekId = WeekDTO.CreateWithWeekContaining(msg.Model.ExpenseOnDate).WeekId.Value;
                //NOTE DateVehicleFirstUsed has a valid Month day year, but the time part is very very wrong
                //we'll adjust here so we don't drift a day when doing TimeZone Conversion
                existing.ExpenseOnDate = new DateTimeOffset(msg.Model.ExpenseOnDate.Year,
                                                                   msg.Model.ExpenseOnDate.Month,
                                                                   msg.Model.ExpenseOnDate.Day,
                                                                   0, 0, 0, new TimeSpan());      
            }
            var updateMsg = UpdateMessage.CreateFrom(msg, existing);
            var mapped = _mapper.Map<DataAccess.EF.TimeAndExpenceExpenditure>(updateMsg);
            
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<TimeAndExpenceExpenditure>(saved));
        }
    }
}

