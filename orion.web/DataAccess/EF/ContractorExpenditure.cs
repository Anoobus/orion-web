﻿using System;
namespace Orion.Web.DataAccess.EF
{
    public class ContractorExpenditure
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset ExpensedOn { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public int WeekId { get; set; }
        public string CompanyName { get; set; }
        public string OrionPONumber { get; set; }
        public decimal TotalPOContractAmount { get; set; }
    }
}
