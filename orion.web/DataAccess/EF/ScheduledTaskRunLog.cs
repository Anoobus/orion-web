using System;

namespace Orion.Web.DataAccess.EF
{
    public class ScheduleTaskRunLog
    {
        public int ScheduleTaskRunLogId { get; set; }
        public int ScheduleTaskId { get; set; }
        public DateTimeOffset TaskCompletionDate { get; set; }
        public virtual ScheduleTask ScheduleTask { get; set; }
    }
}
