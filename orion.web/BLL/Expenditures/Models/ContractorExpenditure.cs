using System;
namespace Orion.Web.Api.Expenditures.Models
{
    public class ContractorExpenditure : EditableContractorExpenditure
    {
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int WeekId { get; set; }
    }

    public class EditableContractorExpenditure
    {
        public string CompanyName { get; set; }
        public string OrionPONumber { get; set; }
        public decimal TotalPOContractAmount { get; set; }
        public DateTimeOffset ExpensedOn { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
    }
}
