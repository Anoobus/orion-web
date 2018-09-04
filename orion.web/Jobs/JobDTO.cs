using orion.web.Clients;
using orion.web.JobsTasks;
using System.ComponentModel;

namespace orion.web.Jobs
{   
      
    public class JobDTO
    {
        public int JobId { get; set; }
        public ClientDTO Client { get; set; }
        [DisplayName("Code")]        
        public string JobCode { get; set; }
        [DisplayName("Name")]
        public string JobName { get; set; }            
        public SiteDTO Site { get; set; }
        [DisplayName("Job Code")]
        public string FullJobCode => $"{Client?.ClientCode} {JobCode}";
        [DisplayName("Job")]
        public string FullJobCodeWithName => $"{Client?.ClientCode}{JobCode}-{JobName}";
        public TaskCategoryId AllowedCategory { get; set; }

        public decimal TargetHours { get; set; }

    }
}
