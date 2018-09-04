using System;

namespace orion.web.TimeEntries
{
    public class WeekTimeEntry
    {
        public  int WeekId { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
    //public class WeekTimeEntryDetail : WeekTimeEntry
    //{
         
    //    public IEnumerable<JobEffortViewModel> Monday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Tuesday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Wednesday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Thursday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Friday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Saturday { get; set; }
    //    public IEnumerable<JobEffortViewModel> Sunday { get; set; }
    //    public IEnumerable<string> Names { get; set; }
    //    public IEnumerable<TaskDTO> Tasks { get; set; }

    //    public decimal GetHourTotal(IEnumerable<JobEffortViewModel> jobInfo)
    //    {
    //        return jobInfo.Sum(x => x.Values.Sum(z => z.Hours));
    //    }
    //}

    //public class JobEffortViewModel
    //{
    //    public string JobName { get; set; }
    //    public IEnumerable<EffortViewModel> Values { get; set; }        
    //}

    //public class EffortViewModel
    //{
    //    public decimal Hours { get; set; }
    //    public int? SelectedTaskId { get; set; }
    //    public int? JobId { get; set; }
    //    public IEnumerable<TaskDTO> AvailableTasks { get; set; }
    //}
}
