using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Orion.Web.BLL.ArcFlashExpenditureExpenses;
using Orion.Web.BLL.AutoMapper;
using Orion.Web.DataAccess;

namespace Orion.Web.test.BLL
{

    public static class TestAutoMapper
    {
        public static IMapper Instance { get; private set; }
        static TestAutoMapper()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<OrionProfile>());
            Instance = configuration.CreateMapper();
        }
    }

    [TestClass]
    public class AutoMapperProfileShould
    {
        [TestMethod]
        public void BeValid()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<OrionProfile>());
            configuration.AssertConfigurationIsValid();
        }
    }
}

