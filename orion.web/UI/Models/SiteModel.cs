using System.ComponentModel;

namespace Orion.Web.UI.Models
{
    public class SiteModel
    {
        public int SiteID { get; set; }

        [DisplayName("Site Name")]
        public string SiteName { get; set; }
    }
}
