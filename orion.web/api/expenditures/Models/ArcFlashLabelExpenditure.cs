using System;
using Microsoft.AspNetCore.Mvc;
using orion.web.BLL;

namespace orion.web.api.expenditures.Models
{
     public class ArcFlashLabelExpenditureOneTimeSet : EditableArcFlashLabelExpenditure
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

    public class ArcFlashLabelExpenditure
        : ArcFlashLabelExpenditureOneTimeSet
    {
        public ActionResult AsActionResult()
        {
            return new OkObjectResult(this);
        }
    }
}

