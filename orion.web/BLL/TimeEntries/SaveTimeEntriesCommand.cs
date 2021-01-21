using orion.web.Common;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ISaveTimeEntriesCommand
    {
        Task<CommandResult> SaveTimeEntriesAsync(int employeeId, int id, FullTimeEntryViewModel vm);
    }
    public class SaveTimeEntriesCommand : ISaveTimeEntriesCommand, IAutoRegisterAsSingleton
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly ITimeService timeService;

        public SaveTimeEntriesCommand(ITimeApprovalService timeApprovalService, ITimeService timeService)
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
        }
        public async Task<CommandResult> SaveTimeEntriesAsync(int employeeId, int id, FullTimeEntryViewModel vm)
        {
            var statusAllowsSave = await GetSaveStatus(employeeId, id);
            if(!statusAllowsSave)
            {
                return new CommandResult(false, "Current status does not allow saving");
            }
            var hoursIssues = VerifyHourEntries(vm).ToArray();
            if(hoursIssues.Any())
            {
                return new CommandResult(false, hoursIssues);
            }


            var timeEntries = await timeService.GetAsync(id, employeeId);

            if(vm.TimeEntryRow != null)
            {
                foreach(var JobTaskCombo in vm.TimeEntryRow)
                {
                    foreach(var day in JobTaskCombo.AllDays())
                    {
                        var match = timeEntries.FirstOrDefault(x => x.TimeEntryId == day.TimeEntryId);
                        match.Hours = day.Hours;
                        match.OvertimeHours = day.OvertimeHours;
                        await timeService.SaveAsync(id, employeeId, match);
                    }
                }
            }

            return new CommandResult(true);
        }

        private IEnumerable<string> VerifyHourEntries(FullTimeEntryViewModel vm)
        {
            if(vm.TimeEntryRow != null)
            {
                if(vm.TimeEntryRow.Any(z => z.AllDays().Any(x => x.Hours < 0 || x.OvertimeHours < 0)))
                {
                    yield return "Hours must be a non-negative number.";
                }
                if(vm.TimeEntryRow.Any(z => z.AllDays().Any(x => x.Hours + x.OvertimeHours > 24)))
                {
                    yield return "Regular + Overtime must never exceed 24 hours.";
                }
                if(vm.TimeEntryRow.Sum(x => x.AllDays().Sum(z => z.Hours)) > 40)
                {
                    yield return "Regular hours must never exceed 40 hours";
                }
            }
        }

        private async Task<bool> GetSaveStatus(int employeeId, int id)
        {
            var status = await timeApprovalService.GetAsync(id, employeeId);
            var statusAllowsSave = status.TimeApprovalStatus != TimeApprovalStatus.Submitted
                && status.TimeApprovalStatus != TimeApprovalStatus.Approved;
            return statusAllowsSave;
        }
    }
}
