using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeSummaryService : IRegisterByConvention
    {
        Task<TimeSummaryDTO> GetAsync(int yearId, int weekId, int employeeId);
    }
    public class TimeSummaryService : ITimeSummaryService
    {
        private readonly OrionDbContext db;

        public TimeSummaryService(OrionDbContext db)
        {
            this.db = db;
        }

        public async Task<TimeSummaryDTO> GetAsync(int yearId, int weekId, int employeeId)
        {
            
            var listy = await db.TimeEntries.Where(x => x.WeekId == weekId && x.Date.Year == yearId && x.EmployeeId == employeeId).ToListAsync();
            var approvalStatus = await db.TimeSheetApprovals.FirstOrDefaultAsync(x => x.EmployeeId == employeeId
            && x.WeekId == weekId);
            var mapped = string.IsNullOrWhiteSpace(approvalStatus?.TimeApprovalStatus) ? TimeApprovalStatus.Unkown : Enum.Parse<TimeApprovalStatus>(approvalStatus.TimeApprovalStatus);
            var existing = listy.GroupBy(x => x.EmployeeId).Select(x => new TimeSummaryDTO()
            {
                EmployeeId = x.Key,
                Hours = x.Sum(z => z.Hours),
                OvertimeHours = x.Sum(z => z.OvertimeHours),
                WeekId = weekId,
                YearId = yearId,
                ApprovalStatus = mapped,
                ResponseDate = approvalStatus?.ApprovalDate,
                SubmittedDate = approvalStatus?.SubmittedDate
            }).FirstOrDefault();

            return existing ?? new TimeSummaryDTO()
            {
                EmployeeId = employeeId,
                Hours = 0,
                OvertimeHours = 0,
                WeekId = weekId,
                YearId = yearId,
                ApprovalStatus = mapped,
                ResponseDate = approvalStatus?.ApprovalDate,
                SubmittedDate = approvalStatus?.SubmittedDate
            };
        }

    



    }
}
