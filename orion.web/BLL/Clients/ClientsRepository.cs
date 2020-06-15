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
        Task<IEnumerable<ClientDTO>> Get();
        ClientDTO Create(ClientDTO client);
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
        public async Task<IEnumerable<ClientDTO>> Get()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return (await db.Clients.ToListAsync())
                    .Select(x => _mapper.Map<ClientDTO>(x))
                    .OrderBy(x => x.ClientName)
                    .ToList();
            }
        }

        public ClientDTO Create(ClientDTO client)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var newClient = _mapper.Map<Client>(client);
                db.Clients.Add(newClient);
                db.SaveChanges();
                client.ClientId = newClient.ClientId;
                return client;
            }
        }
    }
}
