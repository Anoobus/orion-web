using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.Clients
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> Get();
        ClientDTO Post(ClientDTO client);
    }
    public class ClientService :  IClientService
    {
        private readonly OrionDbContext orionDbContext;

        public ClientService(OrionDbContext orionDbContext)
        {
            this.orionDbContext = orionDbContext;
        }
        public IEnumerable<ClientDTO> Get()
        {
            return orionDbContext.Clients.Select(x => new ClientDTO()
            {
                ClientCode = x.ClientCode,
                ClientId = x.ClientId,
                ClientName = x.ClientName
            }).ToList();
        }

        public ClientDTO Post(ClientDTO client)
        {
            var newClient = new Client()
            {
                ClientName = client.ClientName,
                ClientCode = client.ClientCode
            };
            orionDbContext.Clients.Add(newClient);
            orionDbContext.SaveChanges();
            client.ClientId = newClient.ClientId;
            return client;
        }
    }
}
