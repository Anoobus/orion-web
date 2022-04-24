using orion.web.Common;
using orion.web.Jobs;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class EffortDto
    {
        public int JobId { get; set; }
        public int TaskId { get; set; }
    }
    public class EffortTimeEntryDto : EffortDto
    {
        public Dictionary<DayOfWeek,DayTimeEntryDto> Entries { get; set; }
    }

    public class DayTimeEntryDto
    {
                  public DateTime Date { get; set; }
         public decimal Hours { get; set; }
        public decimal OvertimeHours { get; set; }
    }
   
   
    public interface ISaveTimeEntriesCommand
    {
        Task<Result> SaveTimeEntriesAsync(int employeeId, int weekId, FullTimeEntryViewModel vm);
    }
    public class SaveTimeEntriesCommand : ISaveTimeEntriesCommand, IAutoRegisterAsSingleton
    {
        private readonly ITimeApprovalService _timeApprovalService;
        private readonly ITimeService _timeService;
        private readonly IJobsRepository _jobsRepository;

        public SaveTimeEntriesCommand(ITimeApprovalService timeApprovalService, ITimeService timeService, IJobsRepository jobsRepository)
        {
            _timeApprovalService = timeApprovalService;
            _timeService = timeService;
            _jobsRepository = jobsRepository;
        }
        public async Task<Result> SaveTimeEntriesAsync(int employeeId, int weekId, FullTimeEntryViewModel vm)
        {
            var statusAllowsSave = await GetSaveStatus(employeeId, weekId);
            if(!statusAllowsSave)
            {
                return new Result(false, "Current status does not allow saving");
            }
            var hoursIssues = (await VerifyHourEntries(vm)).ToList();
            if(hoursIssues.Any())
            {
                hoursIssues.Add(" -- NO CHANGES WHERE SAVED -- ");
                return new Result(false, hoursIssues.ToArray());
            }

            if(vm.TimeEntryRow != null)
            {
                await _timeService.SaveWeekAsync(employeeId, ToDTO(employeeId, weekId, vm));
            }
            return new Result(true);
        }

        private WeekOfTimeDTO ToDTO(int employeeId, int weekId, FullTimeEntryViewModel vm)
        {
            var w = WeekDTO.CreateForWeekId(weekId);
            return new WeekOfTimeDTO()
            {
                WeekId = weekId,
                Monday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Monday), vm.TimeEntryRow, x => x.Monday),
                Tuesday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Tuesday), vm.TimeEntryRow, x => x.Tuesday),
                Wednesday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Wednesday), vm.TimeEntryRow, x => x.Wednesday),
                Thursday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Thursday), vm.TimeEntryRow, x => x.Thursday),
                Friday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Friday), vm.TimeEntryRow, x => x.Friday),
                Saturday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Saturday), vm.TimeEntryRow, x => x.Saturday),
                Sunday = ToDaysEntries(employeeId, weekId, w.GetDateFor(DayOfWeek.Sunday), vm.TimeEntryRow, x => x.Sunday)
            };
        }

        private Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> ToDaysEntries(int employeeId, int weekid, DateTime date, List<TimeEntryViewModel> allData, Func<TimeEntryViewModel, TimeSpentViewModel> DaySelector)
        {
            var dayEntriesGroupByJobWithTask = allData.GroupBy(x => new JobWithTaskDTO() { JobId = x.SelectedJobId.Value, TaskId = x.SelectedTaskId.Value, }, x => new TimeEntryBaseDTO()
            {
                Date = date,
                Hours = DaySelector(x).Hours,
                OvertimeHours = DaySelector(x).OvertimeHours,
                EmployeeId = employeeId,
                JobId = x.SelectedJobId.Value,
                JobTaskId = x.SelectedTaskId.Value,
                WeekId = weekid
            });

            return dayEntriesGroupByJobWithTask.ToDictionary(x => x.Key, x => new TimeEntryBaseDTO()
            {
                Date = x.FirstOrDefault().Date,
                EmployeeId = employeeId,
                Hours = x.Sum(h => h.Hours),
                JobId = x.FirstOrDefault().JobId,
                JobTaskId = x.FirstOrDefault().JobTaskId,
                OvertimeHours = x.Sum(h => h.OvertimeHours),
                WeekId = x.FirstOrDefault().WeekId
            });
        }

        private async Task<IEnumerable<string>> VerifyHourEntries(FullTimeEntryViewModel vm)
        {
            var issues = new List<String>();
            if(vm.TimeEntryRow != null)
            {
                if(vm.TimeEntryRow.Any(z => z.AllDays().Any(x => x.Hours < 0 || x.OvertimeHours < 0)))
                {
                    issues.Add("Hours must be a non-negative number.");
                }
                if(vm.TimeEntryRow.Any(z => z.AllDays().Any(x => x.Hours + x.OvertimeHours > 24)))
                {
                    issues.Add("Regular + Overtime must never exceed 24 hours.");
                }
                if(vm.TimeEntryRow.Sum(x => x.AllDays().Sum(z => z.Hours)) > 40)
                {
                    issues.Add("Regular hours must never exceed 40 hours");
                }


                var entriesByJob = vm.TimeEntryRow.Select(te => new { jobId = te.SelectedJobId.Value, hasTime = te.AllDays().Any(h => h.Hours > 0 || h.OvertimeHours > 0) });
                var grouped = entriesByJob.GroupBy(x => x.jobId);
                foreach(var jobGroup in grouped)
                {
                    var job = await _jobsRepository.GetForJobId(jobGroup.Key);
                    if(job.CoreInfo.JobStatusId != JobStatus.Enabled)
                    {
                        issues.Add($"Job {job.CoreInfo.FullJobCodeWithName} has been closed, either remove this entry, or have an administrator re-open the job");
                    }
                }
            }
            return issues;
        }

        private async Task<bool> GetSaveStatus(int employeeId, int id)
        {
            var status = await _timeApprovalService.GetAsync(id, employeeId);
            var statusAllowsSave = status.TimeApprovalStatus != TimeApprovalStatus.Submitted
                && status.TimeApprovalStatus != TimeApprovalStatus.Approved;
            return statusAllowsSave;
        }
    }
}
