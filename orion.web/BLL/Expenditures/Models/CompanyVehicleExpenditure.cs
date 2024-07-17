using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Orion.Web.BLL;

namespace Orion.Web.Api.Expenditures.Models
{
    public class CompanyVehicleExpenditure : EditableCompanyVehicleExpenditure
    {
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public int WeekId { get; set; }
    }

    public class EditableCompanyVehicleExpenditure
    {
        public DateTimeOffset DateVehicleFirstUsed { get; set; }
        [JsonConverter(typeof(StringEnumConverter), false)]
        public CompanyVehicleDescriptor Vehicle { get; set; }
        public int TotalNumberOfDaysUsed { get; set; }
        public int TotalMiles { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
    }

    public enum CompanyVehicleDescriptor
    {
        Unknown = 0,
        ChevyBlazer,
        GMCTruck,
        ChevyTruck,
    }
   

    public static class CompanyVehicleExtensions
    {
        public static decimal GetTotalCost(this CompanyVehicleExpenditure vehicle)
        {
            if (vehicle == null)
                return 0;

            /*
             * CompanyVehicleExpenditure
             * To calculate cost for the company vehicle,
             * use $125 per day and add $.50 per mile for each mile over 250 per day.
             */

            var perDay = vehicle.TotalNumberOfDaysUsed * 125;

            var mileAllotmentPerDay = vehicle.TotalNumberOfDaysUsed * 250;
            var perMile = Math.Max(vehicle.TotalMiles - mileAllotmentPerDay, 0) * 0.50m;
            return (decimal)perDay + perMile;
        }
    }
}
