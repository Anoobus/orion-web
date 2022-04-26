using System;
namespace orion.web.DataAccess.EF
{



  
    public class MiscExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset ExpensedOn { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }

        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

}

