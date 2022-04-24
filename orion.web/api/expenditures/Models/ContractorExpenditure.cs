using System;
namespace orion.web.api.expenditures.Models
{
     public class ContractorExpenditureOneTimeSet : EditableContractorExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableContractorExpenditure
    {
        public string CompanyName { get; set; }
        public string OrionPONumber { get; set; }
        public decimal TotalPOContractAmount { get; set; }
    }

    public class ContractorExpenditure : ContractorExpenditureOneTimeSet
    {

    }
}

