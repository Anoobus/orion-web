using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace orion.web.test.TestHelpers
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
