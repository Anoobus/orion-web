using System;
namespace Orion.Web.DataAccess.EF
{
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
}
