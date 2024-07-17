using System;
using AutoFixture;
using AutoMapper;
using Orion.Web.BLL;
using Orion.Web.test.BLL;

namespace Orion.Web.test
{
    public abstract class BaseTestcontext
    {
        protected IMapper _mapper = TestAutoMapper.Instance;
        protected Fixture _fixture = new Fixture();
    }
    //public abstract class MessageHandlerTestcontext<TMessageHandler>
    //    where TMessageHandler : IOnProcessEventEmitter
    //{
    //    protected IMapper _mapper = TestAutoMapper.Instance;
    //    protected Fixture _fixture = new Fixture();
    //    public IResult Actual { get; private set; }
    //    public TMessageHandler GetItemUnderTest()
    //    {
    //        var temp = CreateItemUnderTest();
    //        temp.OnProcessComplete += x => Actual = x;
    //        return temp;
    //    }
    //    protected abstract TMessageHandler CreateItemUnderTest();
    //}
}

