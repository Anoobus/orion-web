using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class ReportViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ReportName { get; set; }
        public IEnumerable<string> Columns { get; set; }
        public List<List<string>> ReportData { get; set; }
        public bool ForDownload { get; set; }
        public Microsoft.AspNetCore.Mvc.Rendering.SelectListItem[] AvailableJobs { get; set; }
        public string SelectedJobId { get; set; }
        
    }

}
