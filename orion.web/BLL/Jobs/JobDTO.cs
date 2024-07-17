using Orion.Web.BLL.Jobs;
using Orion.Web.Clients;

namespace Orion.Web.Jobs
{
    public class CoreJobDto : UpdateJobDto
    {
        public string FullJobCodeWithName => $"{JobCode}-{JobName}";

        // public JobStatusDTO JobStatusDTO { get; set; }
        // public ProjectManagerDTO ProjectManager { get; set; }
        // public ClientDTO Client { get; set; }
        // public SiteDTO Site { get; set; }
    }

    public class JobDto
    {
        // public JobStatusDTO JobStatusDTO { get; set; }
        // public ProjectManagerDTO ProjectManager { get; set; }
        public CoreJobDto CoreInfo { get; set; }
        public ClientDTO Client { get; set; }
        public SiteDTO Site { get; set; }
    }
}
