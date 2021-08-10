using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeService
    {
        Task<IEnumerable<TimeEntryDTO>> GetAsync(int weekId, int employeeId);
        Task SaveAsync(int weekId, int employeeId, TimeEntryDTO entry);
        Task SaveWeekAsync(int employeeId, WeekOfTimeDTO entry);
        Task<WeekOfTimeDTO> GetWeekAsync(int employeeId, int weekId);
        Task DeleteAllEntries(int weekId, int taskId, int JobId, int employeeId);
    }
    public class TimeService : ITimeService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;
        private static readonly ILogger _logger = Log.ForContext<TimeService>();

        public TimeService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task DeleteAllEntries(int weekId, int taskId, int JobId, int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var matches = db.TimeEntries.Where(x => x.WeekId == weekId && x.EmployeeId == employeeId &&
                x.JobId == JobId && x.TaskId == taskId);
                db.TimeEntries.RemoveRange(matches);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TimeEntryDTO>> GetAsync(int weekId, int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.TimeEntries.Where(x => x.WeekId == weekId && x.EmployeeId == employeeId).
                Select(x => new TimeEntryDTO()
                {
                    Date = x.Date,
                    EmployeeId = x.EmployeeId,
                    Hours = x.Hours,
                    JobId = x.JobId,
                    OvertimeHours = x.OvertimeHours,
                    JobTaskId = x.TaskId,
                    TimeEntryId = x.TimeEntryId,
                    WeekId = x.WeekId,
                }).ToListAsync();
            }
        }

        public async Task SaveAsync(int weekId, int employeeId, TimeEntryDTO entry)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var forUpdate = await db.TimeEntries.SingleOrDefaultAsync(x => x.Date == entry.Date && x.EmployeeId == employeeId && x.TaskId == entry.JobTaskId && x.JobId == entry.JobId);
                if(forUpdate == null)
                {
                    forUpdate = MapToEF(employeeId, entry);
                    db.TimeEntries.Add(forUpdate);
                }
                forUpdate.Hours = entry.Hours;
                forUpdate.OvertimeHours = entry.OvertimeHours;
                await db.SaveChangesAsync();
            }
        }

        private static TimeEntry MapToEF(int employeeId, TimeEntryBaseDTO entry)
        {
            return new TimeEntry()
            {
                Date = entry.Date,
                EmployeeId = employeeId,
                Hours = entry.Hours,
                JobId = entry.JobId,
                OvertimeHours = entry.OvertimeHours,
                TaskId = entry.JobTaskId,
                WeekId = entry.WeekId
            };
        }

        public async Task SaveWeekAsync(int employeeId, WeekOfTimeDTO entry)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var allForWeek = await db.TimeEntries.Where(x => x.WeekId == entry.WeekId && x.EmployeeId == employeeId).ToListAsync();
                foreach(var oldEntry in allForWeek)
                {
                    _logger.Information($"Clearing old time entry Job:{oldEntry.JobId}, Task:{oldEntry.TaskId}, Hours: [{oldEntry.Hours}/{oldEntry.OvertimeHours}] date:{oldEntry.Date}");
                    db.TimeEntries.Remove(oldEntry);
                }

                foreach(var toSave in entry.AsEnumerable())
                {
                    await db.TimeEntries.AddRangeAsync(MapToEF(employeeId, toSave));
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<WeekOfTimeDTO> GetWeekAsync(int employeeId, int weekId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var allForWeek = await db.TimeEntries.Where(x => x.WeekId == weekId && x.EmployeeId == employeeId).ToListAsync();
                var groupedByDay = allForWeek.GroupBy(x => x.Date.DayOfWeek, proj => (id: new JobWithTaskDTO() { JobId = proj.JobId, TaskId = proj.TaskId },
                                                                                      sourceValue: proj));
                return new WeekOfTimeDTO()
                {
                    Monday = GetDaysValuesOrDefault(DayOfWeek.Monday, groupedByDay),
                    Tuesday = GetDaysValuesOrDefault(DayOfWeek.Tuesday, groupedByDay),
                    Wednesday = GetDaysValuesOrDefault(DayOfWeek.Wednesday, groupedByDay),
                    Thursday = GetDaysValuesOrDefault(DayOfWeek.Thursday, groupedByDay),
                    Friday = GetDaysValuesOrDefault(DayOfWeek.Friday, groupedByDay),
                    Saturday = GetDaysValuesOrDefault(DayOfWeek.Saturday, groupedByDay),
                    Sunday = GetDaysValuesOrDefault(DayOfWeek.Sunday, groupedByDay),
                };
            }
        }

        private Dictionary<JobWithTaskDTO, TimeEntryBaseDTO> GetDaysValuesOrDefault(DayOfWeek day, IEnumerable<IGrouping<DayOfWeek, (JobWithTaskDTO jobTask, TimeEntry savedTimeEntry)>> groupedByDay)
        {
            var jobWithTaskTimeEntriesForTargetDay = new Dictionary<JobWithTaskDTO, TimeEntryBaseDTO>();
            var targetDay = groupedByDay.SingleOrDefault(x => x.Key == day);

            if(targetDay == null)
                return new Dictionary<JobWithTaskDTO, TimeEntryBaseDTO>();
            var groupedByJobTask = targetDay.GroupBy(x => x.jobTask);

            foreach(var jobTaskGroupedTime in groupedByJobTask)
            {
                var timeForDay = new TimeEntryBaseDTO()
                {
                    Date = jobTaskGroupedTime.First().savedTimeEntry.Date,
                    EmployeeId = jobTaskGroupedTime.First().savedTimeEntry.EmployeeId,
                    Hours = jobTaskGroupedTime.Sum(x => x.savedTimeEntry.Hours),
                    OvertimeHours = jobTaskGroupedTime.Sum(x => x.savedTimeEntry.OvertimeHours),
                    JobId = jobTaskGroupedTime.Key.JobId,
                    JobTaskId = jobTaskGroupedTime.Key.TaskId,
                    WeekId = jobTaskGroupedTime.First().savedTimeEntry.WeekId,
                };

                jobWithTaskTimeEntriesForTargetDay.Add(jobTaskGroupedTime.Key, timeForDay);
            }
            return jobWithTaskTimeEntriesForTargetDay;
        }
    }
}
