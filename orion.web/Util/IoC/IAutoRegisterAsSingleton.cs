using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace orion.web.Util.IoC
{
    public static class IAutoRegisterAsSingleton
    {
        public static IServiceCollection AutoRegisterAsSingletonForMarker<TMarkerInterface>(this IServiceCollection serviceCollection, Assembly assemblyToInterrogate)
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
                        serviceCollection.AddSingleton(matchingInterface, concreteType);
                    }
                }
            }

            return serviceCollection;
        }
    }
}
