using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess.EF;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeApprovalService
    {
        Task<TimeApprovalDTO> Get(int yearId, int weekId, string employeeName);
        Task Save(TimeApprovalDTO timeApprovalDTO);
    }
    public class TimeApprovalService : ITimeApprovalService
    {
        private readonly OrionDbContext db;

        public TimeApprovalService(OrionDbContext db)
        {
            this.db = db;
        }
        public async Task<TimeApprovalDTO> Get(int yearId, int weekId, string employeeName)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == employeeName)?.EmployeeId ?? -1;
            
            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.Year == yearId && x.WeekId == weekId && x.EmployeeId == empId);
            if(match == null)
            {
                return new TimeApprovalDTO()
                {
                    EmployeeName = employeeName,
                    WeekId = weekId,
                    Year = yearId,
                    TimeApprovalStatus = TimeApprovalStatus.Unkown.ToString()
                };
            }
            else
            {
                var approver = db.Employees.FirstOrDefault(x => x.EmployeeId == match.ApproverEmployeeId)?.Name ?? "";
                return new TimeApprovalDTO()
                {
                    ApprovalDate = match.ApprovalDate,
                    ApproverName = approver,
                    EmployeeName = employeeName,
                    ResponseReason = match.ResponseReason,
                    TimeApprovalStatus = match.TimeApprovalStatus,
                    WeekId = match.WeekId,
                    Year = match.Year
                };
            }
        }

        public async Task Save(TimeApprovalDTO timeApprovalDTO)
        {
            var empId = db.Employees.FirstOrDefault(x => x.Name == timeApprovalDTO.EmployeeName)?.EmployeeId ?? -1;

            var match = await db.TimeSheetApprovals.SingleOrDefaultAsync(x => x.Year == timeApprovalDTO.Year && x.WeekId == timeApprovalDTO.WeekId && x.EmployeeId == empId);
            if(match == null)
            {
                match = new TimeSheetApproval()
                {
                    EmployeeId = empId,
                    WeekId = timeApprovalDTO.WeekId,
                    Year = timeApprovalDTO.Year,
                };
                db.TimeSheetApprovals.Add(match);
            } 
            var approver = db.Employees.FirstOrDefault(x => x.Name == timeApprovalDTO.ApproverName)?.EmployeeId ?? 0;
            match.ApprovalDate = DateTime.Now;
            match.ApproverEmployeeId = approver;
            match.ResponseReason = match.ResponseReason ?? "" + timeApprovalDTO.ResponseReason;
            match.TimeApprovalStatus = timeApprovalDTO.TimeApprovalStatus;
        }
    }
}
