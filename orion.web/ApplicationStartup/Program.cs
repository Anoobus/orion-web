using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Threading;

namespace orion.web.ApplicationStartup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
      // Add this line:
            .WriteTo.File(
                @"orion.app.logs",
                fileSizeLimitBytes: 1_000_000,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();

            Log.Information("Starting web site");

            var host = CreateWebHostBuilder(args).Build();

            try
            {
                var retryCount = 0;
                var isInitialized = false;
                while (retryCount < 3 && isInitialized == false)
                {
                    try
                    {
                        using (var serviceScope = host.Services.CreateScope())
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
                        isInitialized = true;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error while trying to initialize DB");
                        if (retryCount >= 3)
                        {
                            throw;
                        }
                        Log.Warning("Waiting 25 seconds before retry");
                        Thread.Sleep(25_000);
                    }
                    retryCount++;
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
