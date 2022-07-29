using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class DetailedExpenseForJobReportDTO
    {
        public string JobCode { get; set; }
        public string JobName { get; set; }
        public string SiteName { get; set; }
        public string ClientName { get; set; }        

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }


        public IEnumerable<TimeAndExposeSectionRow> TimeAndExpense { get; set; }
        public IEnumerable<CompanyVehicleSectionRow> CompanyVehicle { get; set; }
        public IEnumerable<SubContractorSectionRow> SubContractor { get; set; }
        public IEnumerable<ArcFlashLabeSectionRow> ArcFlashLabel { get; set; }
        public IEnumerable<MiscSectionRow> Misc { get; set; }
    }

    public class TimeAndExposeSectionRow
    {
        public DateTimeOffset Date { get; set; }
        public string EmployeeFirstLast { get; set; }
        public decimal Cost { get; set; }
    }

    public class CompanyVehicleSectionRow
    {
        public DateTimeOffset Date { get; set; }
        public string Vehicle { get; set; }
        public string EmployeeFirstLast { get; set; }
        public int NumberOfDays { get; set; }
        public int TotalMiles { get; set; }
        public decimal Cost { get; set; }
    }

    public class SubContractorSectionRow
    {
        public string Company { get; set; }
        public string PONumber { get; set; }        
        public decimal ContractAmount { get; set; }
    }

    public class ArcFlashLabeSectionRow
    {
        public DateTimeOffset DateOfInvoice { get; set; }
        public int Quantity { get; set;}
        public decimal LabelCost { get; set; }
        public decimal PostageCost { get; set; }
        public decimal TotalCost { get; set; }
        public string EmployeeName { get; set; }
    }

    public class MiscSectionRow
    {
        public string Description { get; set; }
        public DateTimeOffset ExpensedOn { get; set; }
        public decimal Cost { get; set; }
    }
}
