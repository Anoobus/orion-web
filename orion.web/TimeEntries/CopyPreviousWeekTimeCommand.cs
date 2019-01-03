using orion.web.Common;
using orion.web.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ICopyPreviousWeekTimeCommand : IRegisterByConvention
    {
        Task CopyPreviousWeekTime(int employeeId, int id);
    }
    public class CopyPreviousWeekTimeCommand : ICopyPreviousWeekTimeCommand
{
        //private readonly IWeekService weekService;
        private readonly ITimeService timeService;
        private readonly IEmployeeService employeeService;
        private readonly ITimeSpentRepository timeSpentRepository;

        public CopyPreviousWeekTimeCommand(
            //IWeekService weekService, 
            ITimeService timeService, 
            IEmployeeService employeeService,
            ITimeSpentRepository timeSpentRepository)
        {
            //this.weekService = weekService;
            this.timeService = timeService;
            this.employeeService = employeeService;
            this.timeSpentRepository = timeSpentRepository;
        }
        public async Task CopyPreviousWeekTime(int employeeId,  int id)
        {
            var prev = WeekDTO.CreateForWeekId(id).Previous();
            var timeEntries = await timeService.GetAsync( prev.WeekId.Value, employeeId);
            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var entryForEveryDayOfWeek = timeSpentRepository.CreateEmptyWeekForCombo( id, entry.Key.JobTaskId,entry.Key.JobId, employeeId); ;
                foreach(var day in entryForEveryDayOfWeek)
                {
                    await timeService.SaveAsync( id, employeeId, day);
                }
            }
        }


    }

}
