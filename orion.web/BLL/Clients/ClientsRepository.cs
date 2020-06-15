using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.Clients
{
    public interface IClientsRepository
    {
        IEnumerable<ClientDTO> Get();
        ClientDTO Create(ClientDTO client);
    }
    public class ClientsRepository : IClientsRepository, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public ClientsRepository(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public IEnumerable<ClientDTO> Get()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return db.Clients.Select(x => new ClientDTO()
                {
                    ClientCode = x.ClientCode,
                    ClientId = x.ClientId,
                    ClientName = x.ClientName
                }).OrderBy(x => x.ClientCode).ToList();
            }
        }

        public ClientDTO Create(ClientDTO client)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var newClient = new Client()
                {
                    ClientName = client.ClientName,
                    ClientCode = client.ClientCode
                };
                db.Clients.Add(newClient);
                db.SaveChanges();
                client.ClientId = newClient.ClientId;
                return client;
            }
        }
    }
}
