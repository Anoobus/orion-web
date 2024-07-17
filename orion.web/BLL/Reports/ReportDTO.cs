using System.Collections.Generic;
using System.Data;

namespace Orion.Web.Reports
{
    public class ReportDTO<TReportBody>
    {
        public string ReportName { get; set; }
        public TReportBody Data { get; set; }
        public Dictionary<string, string> RunSettings { get; set; }
    }
}
