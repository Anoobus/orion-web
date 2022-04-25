using System;
namespace orion.web.api.expenditures.Models
{
  public class MiscExpenditureOneTimeSet : EditableMiscExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableMiscExpenditure
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class MiscExpenditure : MiscExpenditureOneTimeSet
    {

    }
}

