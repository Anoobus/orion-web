using System;
namespace orion.web.BLL.Expenditures
{
    public enum ExpenditureTypeEnum
  {
    ArcFlashLabelExpenditure,
    MiscExpenditure,
    ContractorExpenditure,
    TimeAndExpenceExpenditure,
    CompanyVehicleExpenditure
  }
    public class CoreExpenditureDto
    {
        public ExpenditureTypeEnum ExpenditureType { get; set; }
        public Guid ExternalId { get; set; }        
    }
}

