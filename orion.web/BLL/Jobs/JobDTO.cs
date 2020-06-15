using orion.web.BLL.Jobs;
using orion.web.Clients;

namespace orion.web.Jobs
{

    public class JobDTO : UpdateJobDto
    {
        public string FullJobCodeWithName => $"{JobCode}-{JobName}";
        //public JobStatusDTO JobStatusDTO { get; set; }
        //public ProjectManagerDTO ProjectManager { get; set; }
        //public ClientDTO Client { get; set; }
        //public SiteDTO Site { get; set; }
    }
}
