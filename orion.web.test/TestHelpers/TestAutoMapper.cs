using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Orion.Web.BLL.AutoMapper;

namespace Orion.Web.test.TestHelpers
{
    public static class TestAutoMapper
    {
        public static IMapper Instance { get; private set; }
        static TestAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrionProfile>();
            });
            Instance = config.CreateMapper();
        }
    }
}
