using System;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.BLL;

namespace Orion.Web.Api.Expenditures.Models
{
    public class ArcFlashLabelExpenditure : EditableArcFlashLabelExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int WeekId { get; set; }
    }

    public class EditableArcFlashLabelExpenditure
    {
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public DateTimeOffset DateOfInvoice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalLabelsCost { get; set; }
        public decimal TotalPostageCost { get; set; }
    }
}
