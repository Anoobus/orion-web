using System;
using System.Threading.Tasks;
using AutoMapper;
using orion.web.api;
using orion.web.DataAccess;

namespace orion.web.BLL.ArcFlashExpenditureExpenses
{
    public class CreatArcFlashExpenditure : IMessage<ArcFlashlabelExpenditure>
    {

    }

    public class CreateArcFlashExpenditure : HandleBase<CreatArcFlashExpenditure,ArcFlashlabelExpenditure>
    {
        private readonly IArcFlashLabelExpenditureRepo _repo;
        private readonly IMapper _mapper;
       
        public CreateArcFlashExpenditure(IArcFlashLabelExpenditureRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected override async Task<IResult> Handle(CreatArcFlashExpenditure msg)
        {
            var mapped = _mapper.Map<DataAccess.EF.ArcFlashlabelExpenditure>(msg);
            var saved = await _repo.SaveEntity(mapped);
            return _mapper.Map<ArcFlashlabelExpenditure>(saved);
        }
    }
}

