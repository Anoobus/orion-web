using System;
using AutoFixture;
using AutoMapper;
using orion.web.BLL;
using orion.web.test.BLL;

namespace orion.web.test
{
    public abstract class MessageHandlerTestcontext<TMessageHandler>
        where TMessageHandler : IOnProcessEventEmitter
    {
        protected IMapper _mapper = TestAutoMapper.Instance;
        protected Fixture _fixture = new Fixture();
        public IResult Actual { get; private set; }
        public TMessageHandler GetItemUnderTest()
        {
            var temp = CreateItemUnderTest();
            temp.OnProcessComplete += x => Actual = x;
            return temp;
        }
        protected abstract TMessageHandler CreateItemUnderTest();
    }
}

