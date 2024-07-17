using System;

namespace Orion.Web.TimeEntries
{
    public class TimeEntryDTO : TimeEntryBaseDTO
    {
        public int TimeEntryId { get; set; }
        public virtual DataAccess.EF.JobTask JobTask { get; set; }
    }

    public class TimeEntryBaseDTO
    {
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public int JobTaskId { get; set; }
        public int WeekId { get; set; }

        public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }

        public DateTime Date { get; set; }
    }
}
