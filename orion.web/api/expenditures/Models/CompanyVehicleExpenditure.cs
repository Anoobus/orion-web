using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using orion.web.BLL;

namespace orion.web.api.expenditures.Models
{
     public class CompanyVehicleExpenditureOneTimeSet : EditableCompanyVehicleExpenditure
    {
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableCompanyVehicleExpenditure
    {
        public DateTimeOffset DateVehicleFirstUsed { get; set; }
        [JsonConverter(typeof(StringEnumConverter), false)]
        public CompanyVehicleDescriptor Vehicle { get; set; }
        public int TotalNumberOfDaysUsed { get; set; }
        public int TotalMiles { get; set; }
    }

    public class CompanyVehicleExpenditure : CompanyVehicleExpenditureOneTimeSet, IResult
    {
        public ActionResult AsActionResult()
        {
            return new OkObjectResult(this);
        }
    }


    public enum CompanyVehicleDescriptor
    {
        Unknown = 0,
        Encore,
        Enclave,
        Truck,
    }
}

