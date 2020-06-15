using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
        using System.Linq;
        using System.Reflection;
        using Microsoft.Extensions.DependencyInjection;

        public static class ServiceCollectionAssemblyRegistrationExtension
        {
            public static void RegisterTypesWithRegisterByConventionMarker(this Assembly assembly, IServiceCollection serviceCollection)
            {
                var allTypes = assembly.GetExportedTypes().ToList();
                var exportedInterfaces = allTypes.Where(x => x.IsInterface).ToList();
                var concreteNonAbstractClass = allTypes.Where(x => !x.IsAbstract).Distinct();
                var typeToInterfaceMap = concreteNonAbstractClass.SelectMany(
                    x => ((TypeInfo)x).ImplementedInterfaces
                        .Where(i => exportedInterfaces.Any(ei => ei.Name == i.Name && ei.Namespace == i.Namespace)
                                    && exportedInterfaces.Any(ei => ei == typeof(IRegisterByConvention)))
                        .Select(z => new { Tinterface = z, Timplementation = x }));

                foreach(var map in typeToInterfaceMap)
                {
                    if(map.Tinterface != null && map.Timplementation != null)
                    {
                        serviceCollection.AddTransient(map.Tinterface, map.Timplementation);
                    }
                }
            }
        }
}
