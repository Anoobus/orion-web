using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.JobsTasks
{
    public enum TaskCategoryId
    {        
        unknown = 0,
        internalOnly = 1,
        normal  = 2
    }
    public class TaskDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }       
        public string Description { get; set; }
        public TaskCategoryId TaskCategory { get; set; }
    }
}
