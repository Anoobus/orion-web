using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    
    public class AllOpenJobSummaryReportDTO
    {
        public Dictionary<string,decimal> ExpenseAmountForJob { get; set; }
        public Dictionary<string,JobSummaryEmployeeCells> Rows { get; set; }
        public Dictionary<int,string> EmployeeIdToNameMap { get; set; }
    }

    public class JobSummaryEmployeeCells 
    {
        private readonly Dictionary<int, decimal> totalHoursByEmployeeId;

        public JobSummaryEmployeeCells(Dictionary<int,decimal> totalHoursByEmployeeId)
        {
            this.totalHoursByEmployeeId = totalHoursByEmployeeId;
        }
        public decimal GetTotalHours(int EmployeeId)
        {
            if(totalHoursByEmployeeId.TryGetValue(EmployeeId, out var hours))
            {
                return hours;
            }
            return default(decimal);
       }              
    }
}
