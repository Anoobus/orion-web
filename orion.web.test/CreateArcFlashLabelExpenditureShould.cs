using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.DataAccess;

namespace orion.web.test.BLL
{   
    [TestClass]
    public class CreateArcFlashLabelExpenditureShould
    {
        private class TestContext : BaseTestcontext//: MessageHandlerTestcontext<CreateArcFlashLabelExpenditure>
        {
            private Mock<IArcFlashLabelExpenditureRepo> _repo = new Mock<IArcFlashLabelExpenditureRepo>();
            public bool WasSaveCalled { get; private set; }
            public CreateArcFlashLabelExpenditureMessage TestMessage { get; } 
            public TestContext()
            {
                TestMessage = _fixture.Create<CreateArcFlashLabelExpenditureMessage>();
                _repo.Setup(x => x.SaveEntity(It.IsAny<DataAccess.EF.ArcFlashLabelExpenditure>()))
                     .ReturnsAsync((DataAccess.EF.ArcFlashLabelExpenditure mdl) =>
                     {
                         mdl.Id = 42;
                         WasSaveCalled = true;
                         return mdl;
                     });
            }
            public CreateArcFlashLabelExpenditure GetItemUnderTest()
            {
                return new CreateArcFlashLabelExpenditure(_repo.Object, _mapper);
            }
            
        }

        [TestMethod]
        public async Task SaveExpenditure()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemUnderTest();
                     
            var actionReslut = await underTest.Process(ctx.TestMessage);

            var actual = actionReslut.Success;

            actual.JobId.Should().Be(ctx.TestMessage.jobId);
            actual.DateOfInvoice.Should().Be(ctx.TestMessage.model.DateOfInvoice);
            actual.EmployeeId.Should().Be(ctx.TestMessage.employeeId);
            actual.TotalLabelsCost.Should().Be(ctx.TestMessage.model.TotalLabelsCost);
            actual.TotalPostageCost.Should().Be(ctx.TestMessage.model.TotalPostageCost);
            actual.WeekId.Should().Be(ctx.TestMessage.weekId);
        }
    }
}

