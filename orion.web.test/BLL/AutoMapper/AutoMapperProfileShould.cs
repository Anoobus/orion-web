using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.AutoMapper;
using orion.web.DataAccess;

namespace orion.web.test.BLL
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

