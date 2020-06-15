using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.JobTasks
{
    public enum TaskCategory
    {
        Unknown = 0,
        CivilStructural = 1,
        Electrical = 2,
        Mechanical = 3,
        Administrative = 4
    }
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TaskCategory Enum { get; set; }
    }
}
