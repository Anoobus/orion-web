using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;
using orion.web.UI;
using orion.web.Util.IoC;
using orion.web.BLL.AutoMapper;
using System;

namespace orion.web.ApplicationStartup
{


    public class Startup
    {
        public static readonly Lazy<string> AppVersion = new Lazy<string>(() =>
        {
            var assm = typeof(Startup).Assembly;
            var name = assm.GetName().Name;
            return $"{name}: {assm.GetName().Version}";
        });

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Serilog.Log.Information($"Running startup for {environment.EnvironmentName}");


        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Serilog.Log.Information("STARTING DB STUFF UP");

            SetupLocalDbBasedEntityFramework<OrionDbContext>(services, OrionDbContext.CONN_STRING_NAME);
            SetupLocalDbBasedEntityFramework<ApplicationDbContext>(services, "IdentityConnection");

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AutoRegisterForMarker<IAutoRegisterAsSingleton>(typeof(Startup).Assembly, ServiceLifetime.Singleton);
            services.AutoRegisterForMarker<IAutoRegisterAsTransient>(typeof(Startup).Assembly, ServiceLifetime.Transient);

            services.Configure<RazorViewEngineOptions>(config => config.ViewLocationExpanders.Add(new ViewLocationExpander()));
            services.AddHttpContextAccessor();
            services.AddCustomAutoMapper();
        }

        private void SetupLocalDbBasedEntityFramework<TContext>(IServiceCollection services, string connectionName) where TContext : DbContext
        {
            try
            {
                var cnb = new SqlConnectionStringBuilder(Configuration.GetConnectionString(connectionName));
                var runningLocation = Path.Combine(new DirectoryInfo(Path.GetDirectoryName(this.GetType().Assembly.Location)).Parent.FullName, @"sql-data");

                Serilog.Log.Information($"Using run location {runningLocation} for {connectionName}");
                cnb.AttachDBFilename = MDFBootstrap.SetupLocalDbFile(Configuration.GetValue<string>("OverrideSqlDataPath") ?? runningLocation, cnb.InitialCatalog);
                Configuration.GetSection("ConnectionStrings")[connectionName] = cnb.ConnectionString;
                Serilog.Log.Information($"{connectionName} DB SETTINGS => {Configuration.GetConnectionString(connectionName)}");
                services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString(connectionName)));
            }
            catch(Exception e)
            {
                Serilog.Log.Error(e, "Died setting up DB");
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Serilog.Log.Information($"Configuring: {env.ApplicationName} for env {env.EnvironmentName}");
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
