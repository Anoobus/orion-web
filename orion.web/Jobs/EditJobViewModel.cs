﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace orion.web.Jobs
{
    public class EditJobViewModel
    {

        public JobDTO Job { get; set; }
        [DisplayName("Status")]
        public int SelectedJobStatusId { get; set; }
        [BindNever]
        public IEnumerable<JobStatusDTO> AvailableJobStatus { get; set; }
    }
}
