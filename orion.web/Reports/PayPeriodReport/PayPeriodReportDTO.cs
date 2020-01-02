using System;
using System.Collections.Generic;

namespace orion.web.Reports
{
    public class PayPeriodReportDTO
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
        public decimal PTO { get; set; }
        public decimal Holiday { get; set; }
        public decimal Combined { get; set; }
    }

}
