﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.Jobs
{
    public class JobStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobStatus Enum { get; set; }
    }
}
