using orion.web.DataAccess.EF;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.TimeEntries
{

  

    public interface ITimeService
    {        
        IEnumerable<TimeEntryDTO> Get(int yearId, int weekId, string employeeName);
        void Save(int yearId, int weekId, string employeeName, TimeEntryDTO entry);
    }
    public class TimeService :  ITimeService
    {
        private readonly OrionDbContext db;

        public TimeService(OrionDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<TimeEntryDTO> Get(int year, int weekId, string employeeName)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == employeeName)?.EmployeeId ?? -1;
            return db.TimeEntries.Where(x => x.WeekId == weekId && x.Date.Year == year && x.EmployeeId == empId).
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
                }).ToList();
        }

        public void Save(int year, int weekId, string employeeName, TimeEntryDTO entry)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == employeeName)?.EmployeeId ?? -1;
            var forUpdate = db.TimeEntries.SingleOrDefault(x => x.Date == entry.Date && x.EmployeeId == empId && x.TaskId == entry.JobTaskId && x.JobId == entry.JobId);
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
            db.SaveChanges();
        }

       

    }
}
