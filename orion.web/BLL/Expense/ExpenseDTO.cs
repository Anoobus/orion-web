using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.Expense
{
    public class ExpenseDTO
    {
        public DateTimeOffset SaveDate { get; set; }
        public decimal Amount { get; set; }
        public string AttatchmentName { get; set; }
        public string Classification { get; set; }
        public string AddtionalNotes { get; set; }
        public Jobs.CoreJobDto RelatedJob { get; set; }
        public int EmployeeId { get; set; }
        public Guid? AttachmentId { get; set; }
        public int WeekId { get; set; }
        public int ExpenseItemId { get; set; }
    }
}
