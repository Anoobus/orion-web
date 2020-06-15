using orion.web.Jobs;

namespace orion.web.BLL.Jobs
{
    public class CreateJobDto
    {
        public int ClientId { get; set; }
        public string JobCode { get; set; }
        public string JobName { get; set; }
        public int SiteId { get; set; }
        public decimal TargetHours { get; set; }
        public JobStatus JobStatusId { get; set; }
        public int ProjectManagerEmployeeId { get; set; }
    }
    public class UpdateJobDto : CreateJobDto
    {
        public int JobId { get; set; }
    }
}
