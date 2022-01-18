using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace orion.web.AspNetCoreIdentity
{

    public class AppIdentMigrationContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Server=localhost\\sql2017;Integrated Security=true;Database=orion.identity;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
           : base(new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseSqlServer(@"Server=localhost\\sql2017;Integrated Security=true;Database=orion.identity;")
                 .Options)
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
