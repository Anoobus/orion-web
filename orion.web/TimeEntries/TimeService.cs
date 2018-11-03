using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeService : IRegisterByConvention
    {        
        Task<IEnumerable<TimeEntryDTO>> GetAsync(int yearId, int weekId, string employeeName);
        Task SaveAsync(int yearId, int weekId, string employeeName, TimeEntryDTO entry);
        Task DeleteAllEntries(int year, int weekId, int taskId, int JobId, string employeeName);
    }
    public class TimeService : ITimeService
    {
        private readonly OrionDbContext db;

        public TimeService(OrionDbContext db)
        {
            this.db = db;
        }

        public async Task DeleteAllEntries(int year, int weekId, int taskId, int JobId, string employeeName)
        {
            var empId = (await db.Employees.FirstOrDefaultAsync(x => x.Name == employeeName))?.EmployeeId ?? -1;
            var matches = db.TimeEntries.Where(x => x.WeekId == weekId && x.Date.Year == year && x.EmployeeId == empId &&
            x.JobId == JobId && x.TaskId == taskId);
            db.TimeEntries.RemoveRange(matches);
            await db.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<TimeEntryDTO>> GetAsync(int year, int weekId, string employeeName)
        {
            var empId = (await db.Employees.FirstOrDefaultAsync(x => x.Name == employeeName))?.EmployeeId ?? -1;
            return await db.TimeEntries.Where(x => x.WeekId == weekId && x.Date.Year == year && x.EmployeeId == empId).
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

        public async System.Threading.Tasks.Task SaveAsync(int year, int weekId, string employeeName, TimeEntryDTO entry)
        {
            var empId = (await db.Employees.FirstOrDefaultAsync(x => x.Name == employeeName))?.EmployeeId ?? -1;
            var forUpdate = await db.TimeEntries.SingleOrDefaultAsync(x => x.Date == entry.Date && x.EmployeeId == empId && x.TaskId == entry.JobTaskId && x.JobId == entry.JobId);
            if (forUpdate == null)
            {
                forUpdate = new TimeEntry()
                {
                    Date = entry.Date,
                    EmployeeId = empId,
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
