using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class PayPeriodDataDTO
    {
        public IEnumerable<PayPeriodEmployees> Employees { get; set; }
        public DateTime PayPeriodState { get; set; }
        public DateTime PayPeriodEnd { get; set; }
    }

    public class PayPeriodEmployees
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsExempt { get; set; }
        public decimal Regular { get; set; }
        public decimal Overtime { get; set; }
        public decimal Vacation { get; set; }
        public decimal Sick { get; set; }
        public decimal Personal { get; set; }
        public decimal Holiday { get; set; }
        public decimal ExcusedNoPay { get; set; }
        public decimal Combined { get; set; }
    }

}
