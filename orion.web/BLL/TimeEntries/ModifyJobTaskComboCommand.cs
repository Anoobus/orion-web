using orion.web.Common;
using orion.web.Employees;
using orion.web.Util.IoC;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface IModifyJobTaskComboCommand
    {
        Task<Result> ModifyJobTaskCombo(int employeeId,  int weekId, int newTaskId, int newJobId, int oldTaskId, int oldJobId);
    }
    public class ModifyJobTaskComboCommand : IModifyJobTaskComboCommand, IAutoRegisterAsSingleton
    {
        private readonly IEmployeeRepository employeeService;
        //private readonly IWeekService weekService;
        private readonly ITimeService timeService;
        private readonly ITimeSpentRepository timeSpentRepository;

        public ModifyJobTaskComboCommand(IEmployeeRepository employeeService,
          //  IWeekService weekService,
            ITimeService timeService,
            ITimeSpentRepository timeSpentRepository)
        {
            this.employeeService = employeeService;
            //this.weekService = weekService;
            this.timeService = timeService;
            this.timeSpentRepository = timeSpentRepository;
        }
        public async Task<Result> ModifyJobTaskCombo(int employeeId,  int weekId, int newTaskId, int newJobId, int oldTaskId, int oldJobId)
        {

            var timeEntries = await timeService.GetAsync( weekId, employeeId);

            var oldEntries = timeEntries.Where(x => x.JobId == oldJobId && x.JobTaskId == oldTaskId).ToList();
            var existingEntries = timeEntries.Where(x => x.JobId == newJobId && x.JobTaskId == newTaskId).ToList();
            foreach(var item in oldEntries.GroupBy(x => x.Date.DayOfWeek).Select(z => new { DayOfWeek = z.Key, entry = z.First() }))
            {

                var match = existingEntries.FirstOrDefault(x => x.Date.DayOfWeek == item.DayOfWeek);
                if(match != null)
                {
                    match.Hours += item.entry.Hours;
                    match.OvertimeHours += item.entry.OvertimeHours;
                    await timeService.SaveAsync( weekId, employeeId, match);
                }
                else
                {
                    await timeService.SaveAsync( weekId, employeeId, new TimeEntryDTO()
                    {
                        Date = item.entry.Date,
                        EmployeeId = item.entry.EmployeeId,
                        Hours = item.entry.Hours,
                        JobId = newJobId,
                        JobTaskId = newTaskId,
                        OvertimeHours = item.entry.OvertimeHours,
                        WeekId = weekId
                    });
                }

            }
            await timeService.DeleteAllEntries( weekId, oldTaskId, oldJobId, employeeId);
            return new Result(true);
        }
    }

}
