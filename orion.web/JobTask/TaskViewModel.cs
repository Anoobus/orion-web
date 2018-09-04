using System.Collections.Generic;

namespace orion.web.JobsTasks
{
    public class TaskViewModel
    {
        public  TaskDTO Task { get; set; }
        public IEnumerable<CategroyDTO> AllTaskCategories { get; set; }  
        public int SelectedCategory { get; set; }
        
    }
    public class CategroyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
