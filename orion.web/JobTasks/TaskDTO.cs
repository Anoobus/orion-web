using orion.web.JobTasks;
using System.ComponentModel.DataAnnotations;

namespace orion.web.JobsTasks
{
    public class TaskDTO
    {
        public int TaskId { get; set; }
        [RegularExpression(@"^[0-9]{4}$",ErrorMessage = "Code must be a 4 digit number")]
        public string LegacyCode { get; set; }
        public string Name { get; set; }       
        public string Description { get; set; }
        public CategoryDTO Category { get; set; }
        public UsageStatusDTO UsageStatus { get; set; }
    }
}
