using System.ComponentModel.DataAnnotations;
using Orion.Web.BLL.JobTasks;
using Orion.Web.JobTasks;

namespace Orion.Web.JobsTasks
{
    public class TaskDTO
    {
        public int TaskId { get; set; }
        [RegularExpression(@"^[0-9]{2}.*$", ErrorMessage = "Code must be at least 2 digits with optional suffix")]
        [Display(Name = "Task Number")]
        public string LegacyCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskReportingTypeDto ReportingType { get; set; }
        public CategoryDTO Category { get; set; }
        public UsageStatusDTO UsageStatus { get; set; }
    }
}
