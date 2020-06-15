using System.Collections.Generic;
using System.Data;

namespace orion.web.Reports
{
    public class ReportDTO<TReportBody>
    {
        public string ReportName { get; set; }
        public TReportBody Data { get; set; }
       public Dictionary<string,string> RunSettings { get; set; }
    }
}
