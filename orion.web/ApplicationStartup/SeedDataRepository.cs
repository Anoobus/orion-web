using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using orion.web.DataAccess.EF;
using orion.web.JobsTasks;
using System.Threading.Tasks;
using orion.web.Employees;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

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
    
            using(var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OrionDbContext>();
                await db.Database.EnsureCreatedAsync();

               
                var dude = new JsonStartupData();
                
          

                foreach(var category in dude.ReadIt<DataAccess.EF.TaskCategory>())
                {
                    if(!db.TaskCategories.Any(x => x.Name == category.Name))
                    {
                        db.TaskCategories.Add(new DataAccess.EF.TaskCategory()
                        {
                            Name = category.Name
                        });
                    }
                }
                await db.SaveChangesAsync();

                foreach(var task in dude.ReadIt<TaskDTO>())
                {
                    if(!db.JobTasks.Any(x => x.ShortName == task.Name))
                    {
                        db.JobTasks.Add(new JobTask()
                        {
                            Description = task.Description,
                            ShortName = task.Name,
                            TaskCategoryId = db.TaskCategories.Single(x => x.Name == task.TaskCategoryName).TaskCategoryId,
                        });
                    }
                }
                await db.SaveChangesAsync();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                
                foreach(var perm in typeof(UserRoleName).GetFields())
                {
                    var permName = perm.GetValue(new UserRoleName()).ToString();
                    if(!db.UserRoles.Any(x => x.Name == permName))
                    {
                        db.UserRoles.Add(new UserRole()
                        {
                            Name = permName
                        });
                    }
                    if((await roleManager.FindByNameAsync(permName)) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(permName));
                    }
                }
                await db.SaveChangesAsync();

                var adminEmail = "admin@company.com";
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var act = await manager.FindByEmailAsync(adminEmail);
                if(act?.Email == null)
                {
                    var pass = config.GetValue<string>("AdminPassword");
                    act = new IdentityUser(adminEmail);
                    await manager.CreateAsync(act, pass);
                }
                if(!await manager.IsInRoleAsync(act, UserRoleName.Admin))
                {
                    await manager.AddToRoleAsync(act, UserRoleName.Admin);
                }

                var adminAccount = db.Employees.FirstOrDefault(x => x.Name == adminEmail);
                
                if(adminAccount == null)
                {
                    adminAccount = new Employee()
                    {
                        Name = adminEmail
                    };
                    
                    db.Employees.Add(adminAccount);

                    adminAccount.UserRoleId = db.UserRoles.Single(x => x.Name == UserRoleName.Admin).UserRoleId;
                }
            
                await db.SaveChangesAsync();
            }
        }
    }
}
