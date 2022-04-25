using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using orion.web.BLL;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.AutoMapper;
using orion.web.BLL.Expenditures;
using orion.web.DataAccess;

namespace orion.web.test.BLL
{
    


    [TestClass]
    public class UpdateCompanyVehicleExpenditureShould
    {
        private class TestContext : BaseTestcontext
        {
            private Mock<ICompanyVehicleExpenditureRepo> _repo = new Mock<ICompanyVehicleExpenditureRepo>();
            public bool WasSaveCalled { get; private set; }
            public UpdateCompanyVehicleExpenditureMessage TestMessage { get; } 
            public TestContext()
            {
                TestMessage = _fixture.Create<UpdateCompanyVehicleExpenditureMessage>();
                 _repo.Setup(x => x.FindByExternalId(It.IsAny<Guid>()))
                     .ReturnsAsync((Guid id) =>
                     {
                         return new DataAccess.EF.CompanyVehicleExpenditure()
                         {
                             ExternalId = id,

                         };
                     });

                _repo.Setup(x => x.SaveEntity(It.IsAny<DataAccess.EF.CompanyVehicleExpenditure>()))
                     .ReturnsAsync((DataAccess.EF.CompanyVehicleExpenditure mdl) =>
                     {
                         mdl.Id = 42;
                         WasSaveCalled = true;
                         return mdl;
                     });
            }
            public UpdateCompanyVehicleExpenditure GetItemUnderTest()
            {
                return new UpdateCompanyVehicleExpenditure(_repo.Object, _mapper);
            }
            
        }

        [TestMethod]
        public async Task SaveExpenditure()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemUnderTest();
                     
            var actionReslut = await underTest.Process(ctx.TestMessage);

            var actual = actionReslut.Success;

           
            actual.Should().NotBeNull();
            ctx.WasSaveCalled.Should().BeTrue();
        }
    }
}

