using System;
namespace orion.web.DataAccess.EF
{



    public class TimeAndExpenceExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public decimal Amount { get; set; }
    }
    public class MiscExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }

        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }


    public class ArcFlashlabelExpenditure
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



    public class CompanyVehicleExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }

        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public DateTimeOffset DateVehicleFirstUsed { get; set; }
        public int CompanyVehicleId { get; set; }
        public virtual CompanyVehicle CompanyVehicle { get; set; }
        public int TotalNumberOfDaysUsed { get; set; }
        public int TotalMiles { get; set; }
    }

    public class CompanyVehicle
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ContractorExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public string CompanyName { get; set; }
        public string OrionPONumber { get; set; }
        public decimal TotalPOContractAmount { get; set; }
    }
}

