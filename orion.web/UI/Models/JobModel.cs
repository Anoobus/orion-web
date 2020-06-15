using orion.web.Clients;
using orion.web.UI.Models;
using System.ComponentModel;

namespace orion.web.Jobs
{

    public class JobModel
    {
        public int JobId { get; set; }
        public ClientModel Client { get; set; }
        [DisplayName("Job Code")]
        public string JobCode { get; set; }
        [DisplayName("Name")]
        public string JobName { get; set; }
        public SiteModel Site { get; set; }
        [DisplayName("Job")]
        public string FullJobCodeWithName => $"{JobCode}-{JobName}";
        public decimal TargetHours { get; set; }
        [DisplayName("Status")]
        public JobStatusModel JobStatusDTO { get; set; }
        [DisplayName("Project Manager")]
        public ProjectManagerModel ProjectManager { get; set; }
    }
}
