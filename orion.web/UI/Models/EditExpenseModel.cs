using System;
using System.Collections.Generic;
using orion.web.api.expenditures.Models;
using orion.web.BLL.Expenditures;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.UI.Models;
namespace orion.web.UI.Models
{

    public class EditExpenseModel
    {
        public ExpenditureTypeEnum ExpenseType { get; set; }
        public Expense<ArcFlashLabelExpenditure> ArcFlashLabelExpenditure { get; set; }
        public Expense<MiscExpenditure> MiscExpenditure { get; set; }
        public Expense<ContractorExpenditure> ContractorExpenditure { get; set; }
        public Expense<TimeAndExpenceExpenditure> TimeAndExpenceExpenditure { get; set; }
        public Expense<CompanyVehicleExpenditure> CompanyVehicleExpenditure { get; set; }
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public IEnumerable<CoreEmployeeDto> AvailableEmployees { get; set; }

        public SharedExpenseModel BasicInfo => this.ExpenseType switch
        {
            ExpenditureTypeEnum.ArcFlashLabelExpenditure => this.ArcFlashLabelExpenditure,
            ExpenditureTypeEnum.CompanyVehicleExpenditure => this.CompanyVehicleExpenditure,
            ExpenditureTypeEnum.ContractorExpenditure => this.ContractorExpenditure,
            ExpenditureTypeEnum.MiscExpenditure => this.MiscExpenditure,
            _ => throw new NotImplementedException()
        };
  }
}