using orion.web.Common;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ISaveTimeEntriesCommand : IRegisterByConvention
    {
        Task<CommandResult> SaveTimeEntriesAsync(string employeeName, int year, int id, FullTimeEntryViewModel vm);
    }
    public class SaveTimeEntriesCommand : ISaveTimeEntriesCommand
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly ITimeService timeService;

        public SaveTimeEntriesCommand(ITimeApprovalService timeApprovalService, ITimeService timeService  )
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
        }
        public async Task<CommandResult> SaveTimeEntriesAsync(string employeeName, int year, int id, FullTimeEntryViewModel vm)
        {
            var statusAllowsSave = await GetSaveStatus(employeeName, year, id);
            if(statusAllowsSave)
            {
                var timeEntries = await timeService.GetAsync(year, id, employeeName);

                if(vm.TimeEntryRow != null)
                {
                    foreach(var JobTaskCombo in vm.TimeEntryRow)
                    {
                        foreach(var day in JobTaskCombo.AllDays())
                        {
                            var match = timeEntries.FirstOrDefault(x => x.TimeEntryId == day.TimeEntryId);
                            match.Hours = day.Hours;
                            match.OvertimeHours = day.OvertimeHours;
                            await timeService.SaveAsync(year, id, employeeName, match);
                        }
                    }
                }
                return new CommandResult(true);
            }
            else
            {
                return new CommandResult(false, "Current status does not allow saving");
            }
        }

        private async Task<bool> GetSaveStatus(string employeeName, int year, int id)
        {
            var status = await timeApprovalService.Get(year, id, employeeName);
            var statusAllowsSave = status.TimeApprovalStatus != TimeApprovalStatus.Submitted
                && status.TimeApprovalStatus != TimeApprovalStatus.Approved;
            return statusAllowsSave;
        }
    }
}
