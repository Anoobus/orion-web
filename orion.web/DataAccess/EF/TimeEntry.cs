using System;

namespace Orion.Web.DataAccess.EF
{
    public class TimeEntry
    {
        public int TimeEntryId { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int TaskId { get; set; }
        public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }
        public DateTime Date { get; set; }
        public int WeekId { get; set; }
    }
}
