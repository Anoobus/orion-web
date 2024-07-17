using System.Collections.Generic;
using Orion.Web.BLL.JobTasks;
using Orion.Web.JobTasks;

namespace Orion.Web.JobsTasks
{
    public class TaskViewModel
    {
        public TaskDTO Task { get; set; }

        public bool IsInCreateModel { get; set; }
        public IEnumerable<CategoryDTO> AllTaskCategories { get; set; }
        public int SelectedCategory { get; set; }

        public IEnumerable<UsageStatusDTO> AllUsageStatusOptions { get; set; }
        public int SelectedUsageStatus { get; set; }

        public IEnumerable<TaskReportingTypeDto> AllTaskReportingTypes { get; set; }
        public int SelectedTaskReportingType { get; set; }
    }
}
