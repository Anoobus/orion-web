using System;
namespace Orion.Web.Api.Expenditures.Models
{
    public class TimeAndExpenceExpenditure : EditableTimeAndExpenceExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public int WeekId { get; set; }
    }

    public class EditableTimeAndExpenceExpenditure
    {
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseOnDate { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
    }
}
