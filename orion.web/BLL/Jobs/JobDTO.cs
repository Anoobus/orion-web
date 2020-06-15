using orion.web.Clients;
using orion.web.JobsTasks;
using orion.web.JobTasks;
using System.ComponentModel;

namespace orion.web.Jobs
{

    public class JobDTO
    {
        public int JobId { get; set; }
        public ClientDTO Client { get; set; }
        public string JobCode { get; set; }
        public string JobName { get; set; }
        public SiteDTO Site { get; set; }
        public string FullJobCodeWithName => $"{JobCode}-{JobName}";
        public decimal TargetHours { get; set; }
        public JobStatusDTO JobStatusDTO { get; set; }
        public ProjectManagerDTO ProjectManager { get; set; }
    }
}
