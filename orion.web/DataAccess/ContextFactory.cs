using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Orion.Web.DataAccess.EF;
using Orion.Web.Util.IoC;

namespace Orion.Web.DataAccess
{
    public interface IContextFactory
    {
        OrionDbContext CreateDb();
    }

    public class ContextFactory : IContextFactory, IAutoRegisterAsSingleton
    {
        private Lazy<DbContextOptions<OrionDbContext>> _dbOptions;

        public ContextFactory(IConfiguration configuration)
        {
            _dbOptions = new Lazy<DbContextOptions<OrionDbContext>>(() =>
            {
                var optsBuilder = new DbContextOptionsBuilder<OrionDbContext>();

                optsBuilder.UseSqlServer(configuration.GetConnectionString(OrionDbContext.CONNSTRINGNAME));
                return optsBuilder.Options;
            });
        }

        public OrionDbContext CreateDb()
        {
            return new OrionDbContext(_dbOptions.Value);
        }
    }
}
