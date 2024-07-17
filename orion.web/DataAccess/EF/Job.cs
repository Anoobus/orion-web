using System.ComponentModel.DataAnnotations.Schema;

namespace Orion.Web.DataAccess.EF
{
    public class Job
    {
        public int JobId { get; set; }
        public string JobCode { get; set; }
        public string JobName { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int SiteId { get; set; }
        public Site Site { get; set; }

        public decimal TargetHours { get; set; }
        public int JobStatusId { get; set; }
        public virtual JobStatus JobStatus { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee ProjectManager { get; set; }
    }
}
