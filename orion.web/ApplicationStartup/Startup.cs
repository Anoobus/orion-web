﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using orion.web.AspNetCoreIdentity;
using orion.web.DataAccess.EF;
using Microsoft.AspNetCore.Mvc.Razor;
using orion.web.UI;
using orion.web.Util.IoC;
using orion.web.BLL.AutoMapper;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Serilog;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace orion.web.ApplicationStartup
{


    public class Startup
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext<Startup>();

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

            //Register Serilog for all Microsoft ILogger and ILogger<T>
            //services.AddLogging();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(config =>
            {
                config.Cookie.Name = "TimT.Testy";
                config.LoginPath = "/Identity/Account/Login";
            });



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

            services.AddMvc(opts =>
            {

                opts.EnableEndpointRouting = false;
            });

            services.AutoRegisterForMarker<IAutoRegisterAsSingleton>(typeof(Startup).Assembly, ServiceLifetime.Singleton);
            services.AutoRegisterForMarker<IAutoRegisterAsTransient>(typeof(Startup).Assembly, ServiceLifetime.Transient);

            services.Configure<RazorViewEngineOptions>(config => config.ViewLocationExpanders.Add(new ViewLocationExpander()));
            services.AddHttpContextAccessor();
            services.AddCustomAutoMapper();

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v0", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "orion-timetracking.api",
                    Version = "v0",
                });
                c.DocInclusionPredicate((route, descriptor) =>
                {
                    if(descriptor.RelativePath.StartsWith("orion-api"))
                        return true;

                    return false;
                });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                   Description = "Swashbuckle's AddSecurityDefinition Description...",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "Authorization"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    new string[] { }
                }
                });
            });
            LogEmailConfig();
            Serilog.Log.Information($"Services configuration complete");
        }

        private void LogEmailConfig()
        {
            var emailUser = Configuration.GetValue<string>("EmailUserName");
            var host = Configuration.GetValue<string>("EmailHost");
            var port = Configuration.GetValue<int>("EmailPort");
            Serilog.Log.Information($"Emails will use {JsonConvert.SerializeObject(new { emailUser, host, port })}");
        }

        private void SetupLocalDbBasedEntityFramework<TContext>(IServiceCollection services, string connectionName) where TContext : DbContext
        {
            var connString = Configuration.GetConnectionString(connectionName);
            Serilog.Log.Information($"Using {connString} for {connectionName}");
            services.AddDbContext<TContext>(options => options.UseSqlServer(connString));
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
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch(Exception e)
                    {
                        _logger.Error(e, $"Error while handleing {context?.Request?.Path}");
                        throw;
                    }
                });
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMiddleware<JwtWithCookieAuthMiddleware>();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v0/swagger.json", "orion-timetracking.api");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
