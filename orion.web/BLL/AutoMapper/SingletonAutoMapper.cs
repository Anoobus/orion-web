using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Orion.Web.BLL.AutoMapper
{
    public static class AutoMapperStartupExtensions
    {
        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection serviceCollection)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrionProfile>();
            });
            serviceCollection.AddSingleton<IMapper>(config.CreateMapper());
            return serviceCollection;
        }
    }
}
