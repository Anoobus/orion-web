﻿using System;
using System.Collections.Generic;
using orion.web.api.expenditures.Models;
using orion.web.BLL.Expenditures;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.UI.Models;
namespace orion.web.UI.Models
{

    public class ExpenseViewModel
    {
        public ExpenditureTypeEnum ExpenseType { get; set; }
        public bool IsBrandNewExpenditureCreation { get; set; }
        public bool IsOnSaveFix { get; set; }
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
            ExpenditureTypeEnum.TimeAndExpenceExpenditure => this.TimeAndExpenceExpenditure,
            _ => throw new NotImplementedException()
        };

        public string ExpenseTypeDisplayname => this.ExpenseType switch
        {
            ExpenditureTypeEnum.ArcFlashLabelExpenditure => "Arc Flash Label Expenditure",
            ExpenditureTypeEnum.CompanyVehicleExpenditure => "Company Vehicle Expenditure",
            ExpenditureTypeEnum.ContractorExpenditure => "Contractor Expenditure",
            ExpenditureTypeEnum.MiscExpenditure => "Misc. Expenditure",
            ExpenditureTypeEnum.TimeAndExpenceExpenditure => "Time and Expense Ependiture",
            _ => throw new NotImplementedException()
        };
    }
}