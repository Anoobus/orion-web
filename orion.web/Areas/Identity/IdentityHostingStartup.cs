using System;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Orion.Web.Areas.Identity.IdentityHostingStartup))]
namespace Orion.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
