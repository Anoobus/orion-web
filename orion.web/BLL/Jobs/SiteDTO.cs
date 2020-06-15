using System;
using System.ComponentModel;

namespace orion.web.Jobs
{
    public class SiteDTO : IEquatable<SiteDTO>
    {
        public int SiteID { get; set; }
        public string SiteName { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SiteDTO);
        }

        public bool Equals(SiteDTO other)
        {
            return other != null &&
                   SiteID == other.SiteID &&
                   SiteName == other.SiteName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SiteID, SiteName);
        }
    }
}
