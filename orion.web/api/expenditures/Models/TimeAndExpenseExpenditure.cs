using System;
namespace orion.web.api.expenditures.Models
{
    public class TimeAndExpenceExpenditureOneTimeSet : EditableTimeAndExpenceExpenditure
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }        
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int WeekId { get; set; }
    }


    public class EditableTimeAndExpenceExpenditure
    {
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseOnDate { get; set; }
    }

    public class TimeAndExpenceExpenditure : TimeAndExpenceExpenditureOneTimeSet
    {

    }
}

