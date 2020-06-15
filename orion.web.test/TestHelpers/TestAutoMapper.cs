using AutoMapper;
using orion.web.BLL.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace orion.web.test.TestHelpers
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
