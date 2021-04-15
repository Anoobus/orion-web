using System;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(orion.web.Areas.Identity.IdentityHostingStartup))]
namespace orion.web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}