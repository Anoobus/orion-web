using System.Collections.Generic;

namespace orion.web.TimeEntries
{
    public class FullTimeEntryViewModel
    {
        public WeekIdentifier Week { get; set; }
        public List<TimeEntryViewModel> TimeEntryRow { get; set; }
        public NewJobTaskCombo NewEntry { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeDisplayName { get; set; }
        public TimeApprovalStatus ApprovalStatus { get; set; }
        public string SelectedRowId { get; set; }
        public bool IncludeRowsWithNoEffortAppliedOnCopyPreviousWeekTasks { get; set; }
        public IEnumerable<Expense.ExpenseDTO> Expenses { get; set; }
    }
}
