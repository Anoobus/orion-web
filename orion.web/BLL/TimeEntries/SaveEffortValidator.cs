using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orion.Web.Common;
using Orion.Web.Jobs;
using Orion.Web.TimeEntries;
using Orion.Web.Util.IoC;

namespace Orion.Web.BLL.TimeEntries
{
    public interface ISaveEffortValidator
    {
        Task Validate(Action<string> onError, int employeeId, int weekId, EffortTimeEntryDto[] weekOfTime);
    }

    public class SaveEffortValidator : ISaveEffortValidator, IAutoRegisterAsSingleton
    {
        private readonly ITimeApprovalService _timeApprovalService;
        private readonly IJobsRepository _jobsRepository;

        public SaveEffortValidator(ITimeApprovalService timeApprovalService, IJobsRepository jobsRepository)
        {
            _timeApprovalService = timeApprovalService;
            _jobsRepository = jobsRepository;
        }

        public async Task Validate(Action<string> onError, int employeeId, int weekId, EffortTimeEntryDto[] weekOfTime)
        {
            var statusAllowsSave = await GetSaveStatus(employeeId, weekId);
            if (!statusAllowsSave)
            {
                onError("Current status does not allow saving");
                return;
            }

            foreach (var issue in await VerifyHourEntries(weekOfTime))
            {
                onError(issue);
            }
        }

        private async Task<bool> GetSaveStatus(int employeeId, int id)
        {
            var status = await _timeApprovalService.GetAsync(id, employeeId);
            var statusAllowsSave = status.TimeApprovalStatus != TimeApprovalStatus.Submitted
                && status.TimeApprovalStatus != TimeApprovalStatus.Approved;
            return statusAllowsSave;
        }

        private async Task<IEnumerable<string>> VerifyHourEntries(EffortTimeEntryDto[] weekOfTime)
        {
            var issues = new List<string>();
            if (weekOfTime?.Any() ?? false)
            {
                var regHoursByEffort = new Dictionary<string, decimal>();
                var jobsInUse = new HashSet<int>();
                foreach (var dayEntry in weekOfTime.SelectMany(x => x.Entries.Select(z => new { x.TaskId, x.JobId, z.Key, z.Value.Hours, z.Value.OvertimeHours })))
                {
                    jobsInUse.Add(dayEntry.JobId);
                    if (dayEntry.Hours < 0 || dayEntry.OvertimeHours < 0)
                    {
                        issues.Add("Hours must be a non-negative number.");
                    }

                    if (dayEntry.Hours + dayEntry.OvertimeHours > 24)
                    {
                        issues.Add("Regular + Overtime must never exceed 24 hours.");
                    }

                    var totalKey = $"{dayEntry.JobId}.{dayEntry.TaskId}";
                    if (!regHoursByEffort.TryAdd(totalKey, dayEntry.Hours))
                    {
                        regHoursByEffort[totalKey] += dayEntry.Hours;
                    }
                }

                if (regHoursByEffort.Values.Any(x => x > 40))
                {
                    issues.Add("Regular hours must never exceed 40 hours");
                }

                foreach (var id in jobsInUse)
                {
                    var job = await _jobsRepository.GetForJobId(id);
                    if (job.CoreInfo.JobStatusId != JobStatus.Enabled)
                    {
                        issues.Add($"Job {job.CoreInfo.FullJobCodeWithName} has been closed, either remove this entry, or have an administrator re-open the job");
                    }
                }
            }

            return issues;
        }
    }
}
