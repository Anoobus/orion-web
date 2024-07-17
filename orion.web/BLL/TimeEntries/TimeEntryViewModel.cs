using System.Collections.Generic;
using System.Linq;
using Orion.Web.Jobs;
using Orion.Web.JobsTasks;
using Orion.Web.JobTasks;

namespace Orion.Web.TimeEntries
{
    public class Week<T>
    {
        public T Monday { get; set; }
        public T Tuesday { get; set; }
        public T Wednesday { get; set; }
        public T Thursday { get; set; }
        public T Friday { get; set; }
        public T Saturday { get; set; }
        public T Sunday { get; set; }

        public IEnumerable<T> AllDays()
        {
            yield return Monday;
            yield return Tuesday;
            yield return Wednesday;
            yield return Thursday;
            yield return Friday;
            yield return Saturday;
            yield return Sunday;
        }
    }

    public class NewJobTaskCombo
    {
        public IEnumerable<CoreJobDto> AvailableJobs { get; set; }
        public IEnumerable<CategoryDTO> AvailableCategories
        {
            get
            {
                if (AvailableTasks != null)
                {
                    return AvailableTasks.Select(x => x.Category).Distinct();
                }

                return Enumerable.Empty<CategoryDTO>();
            }
        }

        public IEnumerable<TaskDTO> AvailableTasks { get; set; }
        public int? SelectedJobId { get; set; }
        public int? SelectedTaskId { get; set; }
        public string SelectedTaskCategory { get; set; }
    }

    public class TimeEntryViewModel : Week<TimeSpentViewModel>
    {
        public int? SelectedJobId { get; set; }
        public int? SelectedTaskId { get; set; }
        public string SelectedTaskCategory { get; set; }
        public string RowId => $"{SelectedJobId}.{SelectedTaskId}";
        public string SelectedJobCode { get; set; }
        public string SelectedEntryJobName { get; set; }
        public string SelectedEntryTaskName { get; set; }
    }
}
