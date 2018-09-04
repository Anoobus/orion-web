using System;

namespace orion.web.TimeEntries
{
    public class TimeEntryDTO
    {
        public int TimeEntryId { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int JobTaskId { get; set; }
        public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }
        public DateTime Date { get; set; }
        public int WeekId { get; set; }   
        public virtual DataAccess.EF.JobTask JobTask { get; set; }
    }
}
