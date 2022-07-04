﻿using orion.web.Common;
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
        public async Task CopyPreviousWeekTime(int employeeId, int id)
        {
            var prev = WeekDTO.CreateForWeekId(id).Previous();
            var timeEntries = await _timeService.GetAsync(prev.WeekId.Value, employeeId);
            var currentWeek = await _timeService.GetWeekAsync(employeeId, id);
            
            foreach (var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                if (entry.Any(x => x.Hours > 0 || x.OvertimeHours > 0))
                {
                    var entryForEveryDayOfWeek = _timeSpentRepository.CreateEmptyWeekForCombo(id, entry.Key.JobTaskId, entry.Key.JobId, employeeId); ;
                    foreach (var day in entryForEveryDayOfWeek)
                    {
                        var existing = currentWeek.AsEnumerable().FirstOrDefault(x => x.Date.Day == day.Date.Day
                                                                                        && x.JobId == day.JobId
                                                                                        && x.JobTaskId == day.JobTaskId);

                        if(existing == null)
                        {
                            await _timeService.SaveAsync(employeeId, day);
                        }                            
                    }
                }
            }
        }


    }

}
