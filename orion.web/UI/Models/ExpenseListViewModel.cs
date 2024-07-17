using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Api.Expenditures.Models;
using Orion.Web.BLL;
using Orion.Web.Employees;
using Orion.Web.Jobs;

namespace Orion.Web.UI.Models
{
    public class SharedExpenseModel
    {
        public string FullJobNameWithCode { get; set; }
        public string EmployeeName { get; set; }
        public string ShortExpenseName { get; set; }
        public decimal Amount { get; set; }
        public int JobId { get; set; }
        public Guid Id { get; set; }
        public DateTime LastModifiedDateEst { get; set; }
        public DateTime ExpensedOnDateEst { get; set; }
    }

    public class Expense<TDetail> : SharedExpenseModel
    {
        public TDetail Detail { get; set; }
    }

    public class AllExpendituresModel
    {
        public IEnumerable<Expense<ArcFlashLabelExpenditure>> ArcFlashLabelExpenditures { get; set; } = Enumerable.Empty<Expense<ArcFlashLabelExpenditure>>();
        public IEnumerable<Expense<CompanyVehicleExpenditure>> CompanyVehicleExpenditures { get; set; } = Enumerable.Empty<Expense<CompanyVehicleExpenditure>>();
        public IEnumerable<Expense<ContractorExpenditure>> ContractorExpenditures { get; set; } = Enumerable.Empty<Expense<ContractorExpenditure>>();
        public IEnumerable<Expense<MiscExpenditure>> MiscExpenditures { get; set; } = Enumerable.Empty<Expense<MiscExpenditure>>();
        public IEnumerable<Expense<TimeAndExpenceExpenditure>> TimeAndExpenceExpenditures { get; set; } = Enumerable.Empty<Expense<TimeAndExpenceExpenditure>>();
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
               .OrderByDescending(x => x.ExpensedOnDateEst);
        }
    }
}
