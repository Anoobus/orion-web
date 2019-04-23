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
using orion.web.TimeEntries;
using orion.web.Employees;
using orion.web.Common;
using orion.web.Reports;
using orion.web.DataAccess.EF;

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
            Serilog.Log.Information($"Site DB SETTINGS => {Configuration.GetConnectionString("SiteConnection")}");
            Serilog.Log.Information($"Identity DB SETTINGS => {Configuration.GetConnectionString("IdentityConnection")}");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("IdentityConnection")));



            services.AddDbContext<OrionDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SiteConnection")));

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
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
