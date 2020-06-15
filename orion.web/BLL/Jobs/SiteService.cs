using AutoMapper;
using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Jobs
{
    public interface ISitesRepository
    {
        Task<IEnumerable<SiteDTO>> GetAll();
        Task<int> Create(SiteDTO site);
    }

    public class SitesRepository :  ISitesRepository, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public SitesRepository(IContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SiteDTO>> GetAll()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return (await db.Sites.Select(x => new SiteDTO()
                {
                    SiteID = x.SiteID,
                    SiteName = x.SiteName
                }).ToListAsync()).OrderBy(x => x.SiteName)
                .Select(x => _mapper.Map<SiteDTO>(x)).ToList();
            }
        }

        public async Task<int> Create(SiteDTO site)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var newSite = _mapper.Map<Site>(site);
                db.Sites.Add(newSite);
                await db.SaveChangesAsync();
                return newSite.SiteID;
            }
        }
    }
}
