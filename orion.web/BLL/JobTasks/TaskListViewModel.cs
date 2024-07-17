using System.Collections.Generic;

namespace Orion.Web.JobsTasks
{
    public class TaskListViewModel
    {
        public TaskDTO HeaderHelp { get; set; }
        public IEnumerable<TaskDTO> Tasks { get; set; }
    }
}
