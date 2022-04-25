using System.Threading.Tasks;
using AutoMapper;
using orion.web.api.expenditures.Models;
using orion.web.BLL.Core;
using orion.web.DataAccess;
using orion.web.Util.IoC;

namespace orion.web.BLL.ArcFlashExpenditureExpenses
{
   
    public class CreateArcFlashLabelExpenditureMessage : IProduces<ArcFlashLabelExpenditure>
    {
        public CreateArcFlashLabelExpenditureMessage(EditableArcFlashLabelExpenditure model, int weekId, int employeeId, int jobId)
        {
            this.model = model;
            this.weekId = weekId;
            this.employeeId = employeeId;
            this.jobId = jobId;
        }

        public EditableArcFlashLabelExpenditure model { get; }
        public int weekId { get; }
        public int employeeId { get; }
        public int jobId { get; }
    }

   
    public interface ICreateArcFlashLabelExpenditure
    {
          public Task<IProcessResult<ArcFlashLabelExpenditure>> Process(CreateArcFlashLabelExpenditureMessage msg);
    }
    public class CreateArcFlashLabelExpenditure
        : Orchastrator<CreateArcFlashLabelExpenditureMessage, ArcFlashLabelExpenditure>,
        ICreateArcFlashLabelExpenditure, IAutoRegisterAsSingleton
    {
        private readonly IArcFlashLabelExpenditureRepo _repo;
        private readonly IMapper _mapper;

        public CreateArcFlashLabelExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IProcessResult<ArcFlashLabelExpenditure>> Handle(CreateArcFlashLabelExpenditureMessage msg)
        {
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashLabelExpenditure>(msg);
            var saved = await _repo.SaveEntity(mapped);
            return Success(_mapper.Map<ArcFlashLabelExpenditure>(saved));
        }
    }
}

