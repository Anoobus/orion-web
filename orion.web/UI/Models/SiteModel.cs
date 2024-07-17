using System.ComponentModel;

namespace Orion.Web.Jobs
{
    public class SiteModel
    {
        public int SiteID { get; set; }

        [DisplayName("Site Name")]
        public string SiteName { get; set; }
    }
}
