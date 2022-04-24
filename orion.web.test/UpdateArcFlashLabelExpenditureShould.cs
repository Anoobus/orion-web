using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using orion.web.BLL.Expenditures;
using orion.web.DataAccess;

namespace orion.web.test.BLL
{
    [TestClass]
    public class UpdateArcFlashLabelExpenditureShould
    {
        private class TestContext : MessageHandlerTestcontext<UpdateArcFlashLabelExpenditure>
        {
            private Mock<IArcFlashLabelExpenditureRepo> _repo = new Mock<IArcFlashLabelExpenditureRepo>();
            public bool WasSaveCalled { get; private set; }
            public UpdateArcFlashLabelExpenditureMessage TestMessage { get; } 
            public TestContext()
            {
                TestMessage = _fixture.Create<UpdateArcFlashLabelExpenditureMessage>();

                 _repo.Setup(x => x.FindByExternalId(It.IsAny<Guid>()))
                     .ReturnsAsync((Guid id) =>
                     {
                         return new DataAccess.EF.ArcFlashLabelExpenditure()
                         {
                             ExternalId = id,

                         };
                     });

                _repo.Setup(x => x.SaveEntity(It.IsAny<DataAccess.EF.ArcFlashLabelExpenditure>()))
                     .ReturnsAsync((DataAccess.EF.ArcFlashLabelExpenditure mdl) =>
                     {
                         WasSaveCalled = true;
                         return mdl;
                     });
            }
            protected override UpdateArcFlashLabelExpenditure CreateItemUnderTest()
            {
                return new UpdateArcFlashLabelExpenditure(_repo.Object, _mapper);
            }
            
        }

        [TestMethod]
        public async Task SaveExpenditure()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemUnderTest();
                     
            var actionReslut = await underTest.Process(ctx.TestMessage);

            var actual = ctx.Actual as orion.web.api.expenditures.Models.ArcFlashLabelExpenditure;

            actual.Should().NotBeNull();
            ctx.WasSaveCalled.Should().BeTrue();
        }
    }
}

