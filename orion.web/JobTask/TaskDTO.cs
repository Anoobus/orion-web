using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.JobsTasks
{
    
    public class TaskDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }       
        public string Description { get; set; }
        public string TaskCategoryName { get; set; }
    }
}
