using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.Jobs
{

    public interface ISiteService
    {
        IEnumerable<SiteDTO> Get();
        void Post(SiteDTO site);
    }

    public class SiteService :  ISiteService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public SiteService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public IEnumerable<SiteDTO> Get()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return db.Sites.Select(x => new SiteDTO()
                {
                    SiteID = x.SiteID,
                    SiteName = x.SiteName
                }).OrderBy(x => x.SiteName).ToList();
            }
        }

        public void Post(SiteDTO site)
        {
            using(var db = _contextFactory.CreateDb())
            {
                db.Sites.Add(new Site()
                {
                    SiteName = site.SiteName
                });
                db.SaveChanges();
            }
        }
    }
}
