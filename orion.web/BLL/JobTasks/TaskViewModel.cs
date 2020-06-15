using orion.web.JobTasks;
using System.Collections.Generic;

namespace orion.web.JobsTasks
{
    public class TaskViewModel
    {
        public  TaskDTO Task { get; set; }

        public bool IsInCreateModel { get; set; }
        public IEnumerable<CategoryDTO> AllTaskCategories { get; set; }  
        public int SelectedCategory { get; set; }

        public IEnumerable<UsageStatusDTO> AllUsageStatusOptions { get; set; }
        public int SelectedUsageStatus { get; set; }
    }


}
