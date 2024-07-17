using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.JobTasks
{
    public enum TaskCategory
    {
        Unknown = 0,
        CivilStructural = 1,
        Electrical = 2,
        Mechanical = 3,
        Administrative = 4,
        CADD = 5,
        PaidTimeOff = 6,
        Managerial = 7,
        Misc = 8
    }

    public class CategoryDTO : IEquatable<CategoryDTO>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TaskCategory Enum { get; set; }
        public bool IsInternalCategory { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CategoryDTO);
        }

        public bool Equals(CategoryDTO other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Enum == other.Enum &&
                   IsInternalCategory == other.IsInternalCategory;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
