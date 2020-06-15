using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeService
    {
        Task<IEnumerable<TimeEntryDTO>> GetAsync( int weekId, int employeeId);
        Task SaveAsync( int weekId, int employeeId, TimeEntryDTO entry);
        Task DeleteAllEntries( int weekId, int taskId, int JobId, int employeeId);
    }
    public class TimeService : ITimeService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

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

        public async Task<IEnumerable<TimeEntryDTO>> GetAsync( int weekId, int employeeId)
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

        public async Task SaveAsync( int weekId, int employeeId, TimeEntryDTO entry)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var forUpdate = await db.TimeEntries.SingleOrDefaultAsync(x => x.Date == entry.Date && x.EmployeeId == employeeId && x.TaskId == entry.JobTaskId && x.JobId == entry.JobId);
                if(forUpdate == null)
                {
                    forUpdate = new TimeEntry()
                    {
                        Date = entry.Date,
                        EmployeeId = employeeId,
                        Hours = entry.Hours,
                        JobId = entry.JobId,
                        OvertimeHours = entry.OvertimeHours,
                        TaskId = entry.JobTaskId,
                        WeekId = entry.WeekId
                    };
                    db.TimeEntries.Add(forUpdate);
                }
                forUpdate.Hours = entry.Hours;
                forUpdate.OvertimeHours = entry.OvertimeHours;
                await db.SaveChangesAsync();
            }
        }



    }
}
