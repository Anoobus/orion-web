using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using orion.web.DataAccess.EF;
using System.Threading.Tasks;
using orion.web.Employees;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading;

namespace orion.web.ApplicationStartup
{
    public class SeedDataRepository
    {
        private readonly IServiceProvider services;

        public SeedDataRepository(IServiceProvider services)
        {
            this.services = services;
        }
       
        public async Task IntializeSeedData()
        {
                    await SeedDbData();
        }

        private async Task SeedDbData()
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OrionDbContext>();
                Log.Information("Updating OrionDbContext");
                await db.Database.EnsureCreatedAsync();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var perm in typeof(UserRoleName).GetFields())
                {
                    var permName = perm.GetValue(new UserRoleName()).ToString();
                    if (!db.UserRoles.Any(x => x.Name == permName))
                    {
                        Log.Information("Adding role {0} to Orion.Web DB", perm);
                        db.UserRoles.Add(new UserRole()
                        {
                            Name = permName
                        });
                    }
                    if ((await roleManager.FindByNameAsync(permName)) == null)
                    {
                        Log.Information("Adding role {0} to identity server", perm);
                        await roleManager.CreateAsync(new IdentityRole(permName));
                    }
                }
                await db.SaveChangesAsync();

                var adminEmail = "admin@company.com";
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var act = await manager.FindByNameAsync(adminEmail);
                if (act == null)
                {
                    Log.Information("Admin account not found, creating one");
                    var pass = config.GetValue<string>("AdminPassword");
                    act = new IdentityUser(adminEmail);
                    act.Email = adminEmail;                    
                    await manager.CreateAsync(act, pass);
                }
                if (!await manager.IsInRoleAsync(act, UserRoleName.Admin))
                {
                    Log.Information("Admin account not in admin role, adding it");
                    await manager.AddToRoleAsync(act, UserRoleName.Admin);
                }

                var adminAccount = db.Employees.FirstOrDefault(x => x.UserName == adminEmail);

                if (adminAccount == null)
                {
                    adminAccount = new Employee()
                    {
                        UserName = adminEmail,
                        First = "Admin",
                        Last = "Admin"                         
                    };
                    Log.Information("Admin account missing in Orion.Web");
                    db.Employees.Add(adminAccount);

                    adminAccount.UserRoleId = db.UserRoles.Single(x => x.Name == UserRoleName.Admin).UserRoleId;
                }
                Log.Information("Initialize seed data complete");
                await db.SaveChangesAsync();
            }
        }
    }
}
