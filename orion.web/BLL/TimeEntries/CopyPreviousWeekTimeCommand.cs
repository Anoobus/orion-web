using orion.web.Common;
using orion.web.Employees;
using orion.web.Util.IoC;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ICopyPreviousWeekTimeCommand
    {
        Task CopyPreviousWeekTime(int employeeId, int id);
    }
    public class CopyPreviousWeekTimeCommand : ICopyPreviousWeekTimeCommand, IAutoRegisterAsSingleton
    {
        private readonly ITimeService _timeService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimeSpentRepository _timeSpentRepository;

        public CopyPreviousWeekTimeCommand(
            ITimeService timeService,
            IEmployeeRepository employeeRepository,
            ITimeSpentRepository timeSpentRepository)
        {
            _timeService = timeService;
            _employeeRepository = employeeRepository;
            _timeSpentRepository = timeSpentRepository;
        }
        public async Task CopyPreviousWeekTime(int employeeId,  int id)
        {
            var prev = WeekDTO.CreateForWeekId(id).Previous();
            var timeEntries = await _timeService.GetAsync( prev.WeekId.Value, employeeId);
            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var entryForEveryDayOfWeek = _timeSpentRepository.CreateEmptyWeekForCombo( id, entry.Key.JobTaskId,entry.Key.JobId, employeeId); ;
                foreach(var day in entryForEveryDayOfWeek)
                {
                    await _timeService.SaveAsync( id, employeeId, day);
                }
            }
        }


    }

}
