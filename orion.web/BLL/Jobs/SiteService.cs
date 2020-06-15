using orion.web.DataAccess.EF;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.Jobs
{

    public interface ISiteService
    {
        IEnumerable<SiteDTO> Get();
        void Post(SiteDTO site);
    }

    public class SiteService :  ISiteService
    {
        private readonly OrionDbContext db;

        public SiteService(OrionDbContext db)
        {
            this.db = db;
        }
        public IEnumerable<SiteDTO> Get()
        {
            return db.Sites.Select(x => new SiteDTO()
            {
                SiteID = x.SiteID,
                SiteName = x.SiteName
            }).OrderBy(x => x.SiteName).ToList();
        }

        public void Post(SiteDTO site)
        {           
            db.Sites.Add(new Site()
            {
                SiteName = site.SiteName
            });
            db.SaveChanges();
        }
    }
}
