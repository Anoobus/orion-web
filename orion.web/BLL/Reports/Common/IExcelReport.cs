namespace orion.web.Reports.Common
{
    public class ExcelReport<TCriteria> 
    {
        public ExcelReport(string reportName, TCriteria criteria, bool canView)
        {
            this.ReportName = reportName;
            this.Criteria = criteria;
            this.CanView = canView;
        }
        public ExcelReport()
        {

        }
        public string ReportName { get; set; }

        public TCriteria Criteria { get; set; }

        public bool CanView { get; set; }
    }
}
