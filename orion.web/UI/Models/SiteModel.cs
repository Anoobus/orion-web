using System.ComponentModel;

namespace orion.web.Jobs
{
    public class SiteModel
    {
        public int SiteID { get; set; }

        [DisplayName("Site Name")]
        public string SiteName { get; set; }
    }
}
