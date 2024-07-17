using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Orion.Web.Util.IoC
{
    // public interface IAutoRegisterMarker { }

    // public static class IoC
    // {
    //    public interface AutoRegisterAsSingleton<T> : IAutoRegisterMarker
    //    {

    // }
    //    public interface AutoRegisterAsTransient<T> : IAutoRegisterMarker
    //    {

    // }

    // public static IServiceCollection AddAllRegistrations(this IServiceCollection services)
    //    {

    // foreach(var concreteType in typeof(IoC).Assembly.GetTypes().Where(x => x.IsClass))
    //        {
    //            var generisized = typeof(IoC.AutoRegisterAsTransient<>).MakeGenericType(concreteType);
    //            if(generisized.IsAssignableFrom(concreteType))
    //            {
    //                   serviceCollection.Add(new ServiceDescriptor(matchingInterface, concreteType, lifetime));
    //            }

    // var allInterfaces = concreteType.GetInterfaces();
    //            if(concreteType.IsAssignableTo(typeof(IAutoRegisterMarker)))
    //            {
    //                var matchingInterface = allInterfaces.First(x => x.IsGenericTypeDefinition && x.IsAssignableTo(typeof(IAutoRegisterMarker)));
    //                services.Add(new ServiceDescriptor(matchingInterface, concreteType, lifetime));

    // }
    //            if(allInterfaces.Any(x => x == typeof(IoC.AutoRegisterAsSingleton) || x == typeof(IoC.AutoRegisterAsTransient)))
    //            {
    //                var matchingInterface = allInterfaces.SingleOrDefault(x => string.Equals(x.Name.Substring(1), concreteType.Name));
    //                if(matchingInterface != default)
    //                {

    // }
    //            }
    //        }
    //        return services;
    //    }
    // }
}
