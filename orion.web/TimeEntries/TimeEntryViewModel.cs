using orion.web.Jobs;
using orion.web.JobsTasks;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.TimeEntries
{
    public class TimeEntryViewModel
    {
        
        public int? SelectedJobId { get; set; }
        public int? SelectedTaskId { get; set; }
        public string SelectedTaskCategory { get; set; }
        public int RowId { get; set; }
        public IEnumerable<JobDTO> AvailableJobs { get; set; }
        public IEnumerable<string> AvailableCategories { get
            {
                if(AvailableTasks != null)
                {
                    return AvailableTasks.Select(x => x.TaskCategoryName).Distinct();
                }
                return Enumerable.Empty<string>();
            }
        }
        public IEnumerable<TaskDTO> AvailableTasks { get; set; }
    
        public TimeSpentViewModel Monday { get; set; }
        public TimeSpentViewModel Tuesday { get; set; }
        public TimeSpentViewModel Wednesday { get; set; }
        public TimeSpentViewModel Thursday { get; set; }
        public TimeSpentViewModel Friday { get; set; }
        public TimeSpentViewModel Saturday { get; set; }
        public TimeSpentViewModel Sunday { get; set; }

        public IEnumerable<TimeSpentViewModel> AllDays()
        {
            yield return Monday;
            yield return Tuesday;
            yield return Wednesday;
            yield return Thursday;
            yield return Friday;
            yield return Saturday;
            yield return Sunday;
        }

        public string SelectedEntryJobName()
        {
            var job = AvailableJobs.FirstOrDefault(x => x.JobId == SelectedJobId)?.FullJobCodeWithName;
            return job;
            //var other = AvailableJobs.FirstOrDefault(x => x.JobId == SelectedJobId)?.Site?.SiteName;
            //return $"{job}({other})";

        }

        public string SelectedEntryTaskName()
        {
            return AvailableTasks.FirstOrDefault(x => x.TaskId == SelectedTaskId)?.Name;
        }


    }
}
