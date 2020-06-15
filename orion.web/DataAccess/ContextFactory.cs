using orion.web.DataAccess.EF;
using System;
using orion.web.Util.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace orion.web.DataAccess
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
                optsBuilder.UseSqlServer(configuration.GetConnectionString(OrionDbContext.CONN_STRING_NAME));
                return optsBuilder.Options;
            });
        }

        public OrionDbContext CreateDb()
        {
            return new OrionDbContext(_dbOptions.Value);
        }
    }
}
