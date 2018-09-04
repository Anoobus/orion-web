using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using orion.web.ApplicationStartup;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;

namespace orion.web.ApplicationStartup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using(var serviceScope = host.Services.CreateScope())
            {
                //if the DB is not yet migrated just do it now
                //this can be more elaborate as needed down the road
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();

                //if the DB is not yet migrated just do it now
                //this can be more elaborate as needed down the road
                serviceScope.ServiceProvider.GetService<OrionDbContext>().Database.Migrate();
                var seed = new SeedDataRepository(serviceScope.ServiceProvider);
                seed.IntializeSeedData().Wait();
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
