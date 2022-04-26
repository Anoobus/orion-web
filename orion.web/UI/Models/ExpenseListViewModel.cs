using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures.Models;
using orion.web.BLL;
using orion.web.Employees;
using orion.web.Jobs;

namespace orion.web.UI.Models
{
    public class SharedExpenseModel
    {
        public string FullJobNameWithCode { get; set; }
        public string EmployeeName { get; set; }
        public string ShortExpenseName { get; set; }
        public decimal Amount { get; set; }
        public int JobId { get; set; }
        public Guid Id { get; set; }
        public DateTime LastModifiedDateEst {get; set;}

    }
    public class Expense<TDetail> : SharedExpenseModel
    {
        public TDetail Detail { get; set; }
    }
    public class AllExpendituresModel 
    {
        public IEnumerable<Expense<ArcFlashLabelExpenditure>> ArcFlashLabelExpenditures { get; set; }
        public IEnumerable<Expense<CompanyVehicleExpenditure>> CompanyVehicleExpenditures { get; set; }
        public IEnumerable<Expense<ContractorExpenditure>> ContractorExpenditures { get; set; }
        public IEnumerable<Expense<MiscExpenditure>> MiscExpenditures { get; set; }
        public IEnumerable<Expense<TimeAndExpenceExpenditure>> TimeAndExpenceExpenditures { get; set; }
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }

        public ActionResult AsActionResult()
        {
            return new ObjectResult(this);
        }

        public IEnumerable<SharedExpenseModel> AsFullList()
        {
            return ArcFlashLabelExpenditures.OfType<SharedExpenseModel>()
               .Concat(CompanyVehicleExpenditures.OfType<SharedExpenseModel>())
               .Concat(ContractorExpenditures.OfType<SharedExpenseModel>())
               .Concat(MiscExpenditures.OfType<SharedExpenseModel>())
               .Concat(TimeAndExpenceExpenditures.OfType<SharedExpenseModel>())
               .OrderByDescending(x => x.LastModifiedDateEst);           
        }
    }
    
}

