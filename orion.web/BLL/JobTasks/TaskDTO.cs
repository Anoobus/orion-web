﻿using orion.web.BLL.JobTasks;
using orion.web.JobTasks;
using System.ComponentModel.DataAnnotations;

namespace orion.web.JobsTasks
{
    public class TaskDTO
    {
        public int TaskId { get; set; }
        [RegularExpression(@"^[0-9]{2}.*$", ErrorMessage = "Code must be at least 2 digits with optional suffix")]
        [Display(Name = "Task Number")]
        public string LegacyCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskReportingTypeDto ReportingType { get;set;}
        public CategoryDTO Category { get; set; }
        public UsageStatusDTO UsageStatus { get; set; }
    }
}
