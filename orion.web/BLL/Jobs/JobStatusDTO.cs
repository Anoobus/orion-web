using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.Jobs
{
    public enum JobStatus
    {
        Unkown = 0,
        Enabled = 1,
        Archived = 2,
    }

    public class JobStatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobStatus Enum { get; set; }
    }
}
