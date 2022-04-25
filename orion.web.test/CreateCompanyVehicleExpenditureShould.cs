﻿using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using orion.web.BLL.Expenditures;
using orion.web.DataAccess;

namespace orion.web.test.BLL
{



    [TestClass]
    public class CreateCompanyVehicleExpenditureShould
    {
        private class TestContext : BaseTestcontext
        {
            private Mock<ICompanyVehicleExpenditureRepo> _repo = new Mock<ICompanyVehicleExpenditureRepo>();
            public bool WasSaveCalled { get; private set; }
            public CreateCompanyVehicleExpenditureMessage TestMessage { get; } 
            public TestContext()
            {
                TestMessage = _fixture.Create<CreateCompanyVehicleExpenditureMessage>();
                _repo.Setup(x => x.SaveEntity(It.IsAny<DataAccess.EF.CompanyVehicleExpenditure>()))
                     .ReturnsAsync((DataAccess.EF.CompanyVehicleExpenditure mdl) =>
                     {
                         mdl.Id = 42;
                         WasSaveCalled = true;
                         return mdl;
                     });
            }
            public CreateCompanyVehicleExpenditure GetItemUnderTest()
            {
                return new CreateCompanyVehicleExpenditure(_repo.Object, _mapper);
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
            actual.DateVehicleFirstUsed.Should().Be(ctx.TestMessage.model.DateVehicleFirstUsed);
            actual.EmployeeId.Should().Be(ctx.TestMessage.employeeId);
            actual.TotalMiles.Should().Be(ctx.TestMessage.model.TotalMiles);
            actual.TotalNumberOfDaysUsed.Should().Be(ctx.TestMessage.model.TotalNumberOfDaysUsed);
            actual.WeekId.Should().Be(ctx.TestMessage.weekId);
        }
    }
}

