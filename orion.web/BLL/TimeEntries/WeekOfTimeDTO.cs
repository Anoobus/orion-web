using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.TimeEntries
{
    public class JobWithTaskDTO : IEquatable<JobWithTaskDTO>
    {
        public int JobId { get; set; }
        public int TaskId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as JobWithTaskDTO);
        }

        public bool Equals(JobWithTaskDTO other)
        {
            return other != null &&
                   JobId == other.JobId &&
                   TaskId == other.TaskId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(JobId, TaskId);
        }
    }

    public class WeekOfTimeDTO
    {
        public int WeekId { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Monday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Tuesday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Wednesday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Thursday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Friday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Saturday { get; set; }
        public Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> Sunday { get; set; }

        public IEnumerable<TimeEntryBaseDTO> AsEnumerable()
        {
            foreach (var entry in Monday.Values.Union(Tuesday.Values)
                                              .Union(Wednesday.Values)
                                              .Union(Thursday.Values)
                                              .Union(Friday.Values)
                                              .Union(Saturday.Values)
                                              .Union(Sunday.Values))
            {
                yield return entry;
            }
        }
    }
}
