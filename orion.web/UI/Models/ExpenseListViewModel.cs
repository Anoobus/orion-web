using System;
using System.Collections.Generic;
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
            foreach (var item in ArcFlashLabelExpenditures)
            {
                yield return item;
            }
            foreach (var item in CompanyVehicleExpenditures)
            {
                yield return item;
            }
            foreach (var item in ContractorExpenditures)
            {
                yield return item;
            }
            foreach (var item in MiscExpenditures)
            {
                yield return item;
            }
            foreach (var item in TimeAndExpenceExpenditures)
            {
                yield return item;
            }
        }

        public AllExpendituresModel GetResult()
        {
            return this;
        }
    }
    
}

