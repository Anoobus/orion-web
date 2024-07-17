using System.ComponentModel;
using Orion.Web.Clients;
using Orion.Web.UI.Models;

namespace Orion.Web.Jobs
{
    // public class JobModel
    // {
    //    public int JobId { get; set; }
    //    [DisplayName("Job Code")]
    //    public string JobCode { get; set; }
    //    [DisplayName("Name")]
    //    public string JobName { get; set; }
    //    [DisplayName("Job")]
    //    public string FullJobCodeWithName => $"{JobCode}-{JobName}";
    //    public decimal TargetHours { get; set; }
    //    [DisplayName("Status")]
    //    public JobStatus JobStatusId { get; set; }
    //    [DisplayName("Project Manager")]
    //    public int ProjectManagerEmployeeId { get; set; }

    // }
    public class JobModelDetail
    {
        public ClientModel Client { get; set; }
        public SiteModel Site { get; set; }
        [DisplayName("Status")]
        public JobStatusModel JobStatusDTO { get; set; }
        [DisplayName("Project Manager")]
        public ProjectManagerModel ProjectManager { get; set; }
    }
}
