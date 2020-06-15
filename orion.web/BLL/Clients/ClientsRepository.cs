using AutoMapper;
using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Clients
{
    public interface IClientsRepository
    {
        Task<ClientDTO> GetClient(int ClientId);
        Task<IEnumerable<ClientDTO>> GetAllClients();
        Task<ClientDTO> Save(ClientDTO client);
        Task Delete(int clientId);
    }
    public class ClientsRepository : IClientsRepository, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public ClientsRepository(IContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDTO>> GetAllClients()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return (await db.Clients.ToListAsync())
                    .Select(x => _mapper.Map<ClientDTO>(x))
                    .OrderBy(x => x.ClientName)
                    .ToList();
            }
        }

        public async Task<ClientDTO> Save(ClientDTO client)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var toUpdate = await db.Clients.SingleOrDefaultAsync(x => x.ClientId == client.ClientId);

                if(toUpdate != null)
                {
                    toUpdate.ClientName = client.ClientName;
                    await db.SaveChangesAsync();
                    return _mapper.Map<ClientDTO>(toUpdate);
                }
                else
                {
                    var newClient = _mapper.Map<Client>(client);
                    db.Clients.Add(newClient);
                    db.SaveChanges();
                    client.ClientId = newClient.ClientId;
                    return client;
                }
            }
        }

        public async Task<ClientDTO> GetClient(int ClientId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                return _mapper.Map<ClientDTO>(await db.Clients.SingleOrDefaultAsync(x => x.ClientId == ClientId));
            }
        }

        public async Task Delete(int clientId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var client = await db.Clients.SingleAsync(x => x.ClientId == clientId);
                var inUse = await db.Jobs.AnyAsync(x => x.ClientId == client.ClientId);
                if(!inUse)
                {
                    db.Clients.Remove(client);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
