using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using orion.web.AspNetCoreIdentity;
using orion.web.Clients;
using orion.web.Jobs;
using orion.web.JobsTasks;
using orion.web.Employees;
using orion.web.Common;
using orion.web.Reports;
using orion.web.DataAccess.EF;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;
using orion.web.UI;

namespace orion.web.ApplicationStartup
{


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            SetupLocalDbBasedEntityFramework<OrionDbContext>(services, "SiteConnection");
            SetupLocalDbBasedEntityFramework<ApplicationDbContext>(services, "IdentityConnection");

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IJobSummaryQuery, ProjectStatusReportQuery>();
            services.AddTransient<IUpdateEmployeeCommand, UpdateEmployeeCommand>();
            services.AddTransient<ICreateEmployeeCommand, CreateEmployeeCommand>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IJobService, JobService>();
            services.AddTransient<ISiteService, SiteService>();
            services.AddTransient<ITaskService, TaskService>();
            this.GetType().Assembly.RegisterTypesWithRegisterByConventionMarker(services);

            services.Configure<RazorViewEngineOptions>(config => config.ViewLocationExpanders.Add(new ViewLocationExpander()));
            services.AddHttpContextAccessor();
        }

        private void SetupLocalDbBasedEntityFramework<TContext>(IServiceCollection services, string connectionName) where TContext : DbContext
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Serilog.Log.Information($"Configuring: {env.ApplicationName} for env {env.EnvironmentName}");
            if (env.IsDevelopment())
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
