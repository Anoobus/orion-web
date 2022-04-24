using System;
namespace orion.web.DataAccess.EF
{    
    public class ArcFlashLabelExpenditure
    {
        public Guid ExternalId { get; set; }
        public int Id { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public int EmployeeId { get; set; }
        public int JobId { get; set; }

        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public DateTimeOffset DateOfInvoice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalLabelsCost { get; set; }
        public decimal TotalPostageCost { get; set; }
    }
}

