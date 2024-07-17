using System;
using Microsoft.EntityFrameworkCore;
using Orion.Web.DataAccess;
using Orion.Web.DataAccess.EF;

namespace Orion.Web.test.TestHelpers
{
    public class TestDbFactory : IContextFactory
    {
        private readonly Guid _dbId;

        public TestDbFactory(Guid DbId)
        {
            _dbId = DbId;
        }
        public OrionDbContext CreateDb()
        {
            var builder = new DbContextOptionsBuilder<OrionDbContext>();
            builder.UseInMemoryDatabase(_dbId.ToString());
            return new OrionDbContext(builder.Options);
        }
    }
}
