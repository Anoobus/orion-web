using orion.web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeSpentRepository : IRegisterByConvention
    {
        IEnumerable<TimeEntryDTO> CreateEmptyWeekForCombo(int weekId, int JobTaskId, int JobId, int employeeId);
    }
    public class TimeSpentRepository : ITimeSpentRepository
    {

        private TimeEntryDTO CreateDTO(WeekDTO week, int SelectedTaskId, int SelectedJobId,
          int employeeId, DayOfWeek dayOfWeek)
        {
            var candidate = week.WeekStart;
            while (candidate.DayOfWeek != dayOfWeek)
            {
                candidate = candidate.AddDays(1);
            }
            var dto = new TimeEntryDTO()
            {
                Date = candidate,
                EmployeeId = employeeId,
                Hours = 0,
                OvertimeHours = 0,
                JobId = SelectedJobId,
                JobTaskId = SelectedTaskId,
                TimeEntryId = 0,
                WeekId = week.WeekId.Value
            };
            return dto;
        }

        public IEnumerable<TimeEntryDTO> CreateEmptyWeekForCombo(int weekId, int JobTaskId, int JobId, int employeeId)
        {
            var week = WeekDTO.CreateForWeekId(weekId);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Monday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Tuesday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Wednesday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Thursday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Friday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Saturday);
            yield return CreateDTO(week, JobTaskId, JobId, employeeId, DayOfWeek.Sunday);
        }
    }
}
