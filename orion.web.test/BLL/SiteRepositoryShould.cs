using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orion.Web.Clients;
using Orion.Web.DataAccess.EF;
using Orion.Web.Jobs;
using Orion.Web.test.TestHelpers;

namespace Orion.Web.test.BLL
{
    [TestClass]
    public class SiteRepositoryShould
    {
        private class TestContext
        {
            private static NoRecursionFixture _fixture = new NoRecursionFixture();
            public TestDbFactory DbFactory { get; } = new TestDbFactory(Guid.NewGuid());
            public IEnumerable<SiteDTO> SavedSites { get; private set; }
            public TestContext()
            {
                using (var inMemDb = DbFactory.CreateDb())
                {
                    SavedSites = Enumerable.Range(0, 3)
                                            .Select(z => _fixture.Build<SiteDTO>().With(x => x.SiteID, 0).Create())
                                            .ToList();

                    foreach (var site in SavedSites)
                    {
                        SaveSite(inMemDb, site);
                    }
                }
            }

            private void SaveSite(OrionDbContext inMemDb, SiteDTO dto)
            {
                var site = TestAutoMapper.Instance.Map<Site>(dto);
                inMemDb.Sites.Add(site);
                inMemDb.SaveChanges();
                dto.SiteID = site.SiteID;
            }

            public SitesRepository GetItemToTest()
            {
                return new SitesRepository(DbFactory, TestAutoMapper.Instance);
            }
        }

        [TestMethod]
        public async Task WhenGettingExistingSites_ThenReturnAllSites()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var res = await underTest.GetAll();
            res.Should().BeEquivalentTo(ctx.SavedSites);
        }

        [TestMethod]
        public async Task WhenGetting_Missing_Client_ThenReturnNull()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var newSite = new SiteDTO() { SiteName = Guid.NewGuid().ToString() };
            var siteId = await underTest.Create(newSite);

            using (var db = ctx.DbFactory.CreateDb())
            {
                db.Sites.Any(x => x.SiteID == siteId).Should().BeTrue();
            }
        }
    }
}
