using System.Collections.Generic;

namespace orion.web.JobsTasks
{
    public class TaskListViewModel
    {
        public TaskDTO HeaderHelp { get; set; }
        public  IEnumerable<TaskDTO> Tasks { get; set; }
    }
}
