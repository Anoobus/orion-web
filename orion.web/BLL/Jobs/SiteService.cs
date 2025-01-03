using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orion.Web.DataAccess;
using Orion.Web.DataAccess.EF;
using Orion.Web.Notifications;
using Orion.Web.Util.IoC;

namespace Orion.Web.Jobs
{
    public interface ISitesRepository
    {
        Task<IEnumerable<SiteDTO>> GetAll();
        Task<int> Create(SiteDTO site);
        Task<SiteDTO> GetSite(int siteId);
        Task Delete(int siteId);
        Task Save(SiteDTO siteDTO);
    }

    public class SitesRepository : ISitesRepository, IAutoRegisterAsSingleton
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
            using (var db = _contextFactory.CreateDb())
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
            using (var db = _contextFactory.CreateDb())
            {
                var newSite = _mapper.Map<Site>(site);
                db.Sites.Add(newSite);
                await db.SaveChangesAsync();
                return newSite.SiteID;
            }
        }

        public async Task<SiteDTO> GetSite(int siteId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return _mapper.Map<SiteDTO>(await db.Sites.Where(x => x.SiteID == siteId)
                .Select(x => new SiteDTO()
                {
                    SiteID = x.SiteID,
                    SiteName = x.SiteName
                }).SingleOrDefaultAsync());
            }
        }
        public async Task Delete(int siteId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var toRemove = await db.Sites.Where(x => x.SiteID == siteId).Include(x => x.Jobs).SingleAsync();
                var activeStatusId = (int)JobStatus.Enabled;

                if (toRemove.Jobs.Count > 0 && toRemove.Jobs.Where(x => x.JobStatusId == activeStatusId).Any())
                    throw new InvalidOperationException($"Site has {toRemove.Jobs.Where(x => x.JobStatusId == activeStatusId).Count()} jobs related to it");

                db.Sites.Remove(toRemove);
                await db.SaveChangesAsync();
            }
        }

        public async Task Save(SiteDTO siteDTO)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var temp = await db.Sites.Where(x => x.SiteID == siteDTO.SiteID).SingleAsync();
                temp.SiteName = siteDTO.SiteName;
                await db.SaveChangesAsync();
            }
        }
    }
}
