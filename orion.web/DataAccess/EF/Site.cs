using System.Collections.Generic;

namespace Orion.Web.DataAccess.EF
{
    public class Site
    {
        public Site()
        {
            Jobs = new HashSet<Job>();
        }

        public int SiteID { get; set; }
        public string SiteName { get; set; }

        public ICollection<Job> Jobs { get; set; }
    }
}
