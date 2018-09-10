using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;
using Serilog;
using Serilog.Events;
using System;

namespace orion.web.ApplicationStartup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
          .Enrich.FromLogContext()
          .WriteTo.Console()
      // Add this line:
      .WriteTo.File(
          @"orion.app.logs",
      fileSizeLimitBytes: 1_000_000,
      shared: true,
      flushToDiskInterval: TimeSpan.FromSeconds(1))
          .CreateLogger();
            try
            {
                Log.Information("Starting web site");

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
            catch(Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
