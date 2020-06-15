using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess;
using orion.web.Util.IoC;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public interface ITimeSummaryService
    {
        Task<TimeSummaryDTO> GetAsync(int yearId, int weekId, int employeeId);
    }
    public class TimeSummaryService : ITimeSummaryService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public TimeSummaryService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<TimeSummaryDTO> GetAsync(int yearId, int weekId, int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
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
}
