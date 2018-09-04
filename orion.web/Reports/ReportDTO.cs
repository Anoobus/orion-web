using System.Collections.Generic;

namespace orion.web.Reports
{
    public class ReportDTO
    {
        public string ReportName { get; set; }

        public string[] Columns { get; set; }
        public List<List<string>> ReportData { get; set; }
    }
}
