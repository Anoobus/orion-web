using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.JobTasks
{
    public enum UsageStatus
    {
        Unkown = 0,
        Enabled = 1,
        Disabled = 2,
    }

    public class UsageStatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UsageStatus Enum { get; set; }
    }
}
