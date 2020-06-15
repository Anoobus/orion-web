using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace orion.web.Util.IoC
{
    public static class AutoRegisterExtensions
    {
        public static IServiceCollection AutoRegisterForMarker<TMarkerInterface>(this IServiceCollection serviceCollection, Assembly assemblyToInterrogate, ServiceLifetime lifetime)
        {
            var markerInterface = typeof(TMarkerInterface);

            foreach(var concreteType in assemblyToInterrogate.GetTypes().Where(x => x.IsClass))
            {
                var allInterfaces = concreteType.GetInterfaces();
                if(allInterfaces.Any(x => x == markerInterface))
                {
                    var matchingInterface = allInterfaces.SingleOrDefault(x => string.Equals(x.Name.Substring(1), concreteType.Name));
                    if(matchingInterface != default)
                    {
                        serviceCollection.Add(new ServiceDescriptor(matchingInterface, concreteType, lifetime));
                    }
                }
            }

            return serviceCollection;
        }
    }
}
