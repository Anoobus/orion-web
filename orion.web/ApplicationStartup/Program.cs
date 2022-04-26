using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace orion.web.ApplicationStartup
{
    public class Program
    {
        //public static ILogger Log;
        public static void Main(string[] args)
        {
            var connstring = "Server=localhost\\sql2017;Integrated Security=true;Database=orion.web;";

            


            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                                 .WriteTo.File("serilog-logs/log.txt",
                                                               flushToDiskInterval: TimeSpan.FromSeconds(1),
                                                               shared: true,
                                                               rollingInterval: RollingInterval.Day,
                                                               restrictedToMinimumLevel: LogEventLevel.Information)
                                                  .WriteTo.MSSqlServer(connstring,
                                                               restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                                                               sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions()
                                                               {
                                                                   AutoCreateSqlTable = true,
                                                                   TableName = "AppErrors",
                                                               })
                                                  .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                                                  .CreateLogger();



            Log.Information("Starting web site");

            var host = CreateWebHostBuilder(Log.Logger).Build();
            Log.Information("Web Host Builder Created, start DB Migration");

            try
            {
                var retryCount = 0;
                var isInitialized = false;
                while(retryCount < 2 && isInitialized == false)
                {
                    try
                    {
                        using(var serviceScope = host.Services.CreateScope())
                        {
                            //if the DB is not yet migrated just do it now
                            //this can be more elaborate as needed down the road
                            serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();

                            //if the DB is not yet migrated just do it now
                            //this can be more elaborate as needed down the road
                            serviceScope.ServiceProvider.GetService<OrionDbContext>().Database.Migrate();
                        }
                        isInitialized = true;
                    }
                    catch(Exception e)
                    {
                        Log.Error(e, "Error while trying to initialize DB");
                        if(retryCount >= 2)
                        {
                            throw;
                        }
                        Log.Warning("Waiting 5 seconds before retry");
                        Thread.Sleep(5_000);
                    }
                    retryCount++;
                }
            }
            catch(Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                throw;
            }

            Log.Information("DB Migration, begin app Run..");
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(ILogger logger) =>
            WebHost.CreateDefaultBuilder()
                .UseSerilog(logger)
                .UseStartup<Startup>();

        public static IHostBuilder CreateHostBuilder(string[] args)
       => Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(
               webBuilder => webBuilder.UseStartup<Startup>());
    }
}
