using orion.web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeSpentRepository : IRegisterByConvention
    {
        IEnumerable<TimeEntryDTO> CreateEmptyWeekForCombo(int year, int weekId, int JobTaskId, int JobId, int employeeId);
    }
    public class TimeSpentRepository : ITimeSpentRepository
    {
        private readonly IWeekService weekService;

        public TimeSpentRepository(IWeekService weekService)
        {
            this.weekService = weekService;
        }

        private TimeEntryDTO CreateDTO(int year, int weekId, int SelectedTaskId, int SelectedJobId,
          int employeeId, DayOfWeek dayOfWeek)
        {
            var dto = new TimeEntryDTO()
            {
                Date = weekService.GetWeekDate(year, weekId, dayOfWeek),
                EmployeeId = employeeId,
                Hours = 0,
                OvertimeHours = 0,
                JobId = SelectedJobId,
                JobTaskId = SelectedTaskId,
                TimeEntryId = 0,
                WeekId = weekId
            };
            return dto;
        }

        public IEnumerable<TimeEntryDTO> CreateEmptyWeekForCombo(int year, int weekId, int JobTaskId, int JobId, int employeeId)
        {
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Monday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Tuesday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Wednesday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Thursday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Friday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Saturday);
            yield return CreateDTO(year, weekId, JobTaskId, JobId, employeeId, DayOfWeek.Sunday);
        }
    }
}
