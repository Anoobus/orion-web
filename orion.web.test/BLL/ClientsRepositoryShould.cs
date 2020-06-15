using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using orion.web.Clients;
using orion.web.DataAccess.EF;
using orion.web.test.TestHelpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.test.BLL
{
    [TestClass]
    public class ClientsRepositoryShould
    {
        private class TestContext
        {
            private static NoRecursionFixture _fixture = new NoRecursionFixture();
            public ClientDTO SavedModelNoJob { get; set; }
            public ClientDTO SaveModelWithJob { get; set; }
            public TestDbFactory DbFactory { get; } = new TestDbFactory(Guid.NewGuid());

            public TestContext()
            {
                SavedModelNoJob = _fixture.Build<ClientDTO>().With(x => x.ClientId, 0).Create();
                SaveModelWithJob = _fixture.Build<ClientDTO>().With(x => x.ClientId, 0).Create();
                using(var inMemDb = DbFactory.CreateDb())
                {

                    SaveClient(inMemDb, SaveModelWithJob);
                    SaveClient(inMemDb, SavedModelNoJob);
                    var js = _fixture.Build<JobStatus>().With(x => x.JobStatusId, 1).Create();
                    var site = _fixture.Build<Site>().With(x => x.SiteID, 1).Create();
                    inMemDb.Sites.Add(site);
                    inMemDb.SaveChanges();
                    inMemDb.JobStatuses.Add(js);
                    inMemDb.SaveChanges();
                    inMemDb.Jobs.Add(new Job()
                    {
                        ClientId = SaveModelWithJob.ClientId,
                        JobCode = "01234",
                        JobName = "CoolName",
                        JobStatusId = js.JobStatusId,
                        SiteId = site.SiteID
                    });
                    inMemDb.SaveChanges();
                }
            }

            private void SaveClient(OrionDbContext inMemDb, ClientDTO dto)
            {
                var client = TestAutoMapper.Instance.Map<Client>(dto);
                inMemDb.Clients.Add(client);
                inMemDb.SaveChanges();
                dto.ClientId = client.ClientId;
            }

            public ClientsRepository GetItemToTest()
            {
                return new ClientsRepository(DbFactory, TestAutoMapper.Instance);
            }
        }

        [TestMethod]
        public async Task WhenGettingExistingClient_THenReturnClient()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var res = await underTest.GetClient(ctx.SavedModelNoJob.ClientId);
            res.Should().BeEquivalentTo(ctx.SavedModelNoJob);

            var res2 = await underTest.GetClient(ctx.SaveModelWithJob.ClientId);
            res2.Should().BeEquivalentTo(ctx.SaveModelWithJob);
        }

        [TestMethod]
        public async Task WhenGetting_Missing_Client_ThenReturnNull()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var res = await underTest.GetClient(int.MaxValue);
            res.Should().BeNull();
        }

        [TestMethod]
        public async Task WhenGetAllClients_ThenReturnAllClients()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var res = await underTest.GetAllClients();
            res.Should().Contain(ctx.SavedModelNoJob);
            res.Should().Contain(ctx.SaveModelWithJob);
        }

        [TestMethod]
        public async Task WhenSavingNewClient_ThenInsertClient()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            var newThing = new ClientDTO()
            {
                ClientName = Guid.NewGuid().ToString()
            };

            var final = await underTest.Save(newThing);
            final.ClientId.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task WhenSavingExistingClientClient_ThenUpdateClient()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            ctx.SavedModelNoJob.ClientName = Guid.NewGuid().ToString();

            var actual = await underTest.Save(ctx.SavedModelNoJob);
            actual.ClientName.Should().Be(ctx.SavedModelNoJob.ClientName);
        }

        [TestMethod]
        public async Task WhenDeletingClient_WithClientInUse_ThenDoNothing()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            await underTest.Delete(ctx.SaveModelWithJob.ClientId);

            using(var db = ctx.DbFactory.CreateDb())
            {
                db.Clients.Any(x => x.ClientId == ctx.SaveModelWithJob.ClientId).Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task WhenDeletingClient_WithClient_NOT_InUse_ThenDelete()
        {
            var ctx = new TestContext();
            var underTest = ctx.GetItemToTest();

            await underTest.Delete(ctx.SavedModelNoJob.ClientId);

            using(var db = ctx.DbFactory.CreateDb())
            {
                db.Clients.Any(x => x.ClientId == ctx.SavedModelNoJob.ClientId).Should().BeFalse();
            }
        }

    }
}
