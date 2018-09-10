using System.Collections.Generic;
using System.Data;

namespace orion.web.Reports
{
    public class ReportDTO
    {
        public string ReportName { get; set; }
        public DataTable Data { get; set; }
       public Dictionary<string,string> RunSettings { get; set; }
    }
}
