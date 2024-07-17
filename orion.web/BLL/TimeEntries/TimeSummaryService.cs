using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orion.Web.DataAccess;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeEntries
{
    public interface ITimeSummaryService
    {
        Task<TimeSummaryDTO> GetAsync(int weekId, int employeeId);
    }

    public class TimeSummaryService : ITimeSummaryService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public TimeSummaryService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<TimeSummaryDTO> GetAsync(int weekId, int employeeId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var weekDataByEmployeeId = (await db.WeeklyData.Where(x => x.WeekId == weekId && x.EmployeeId == employeeId).ToListAsync()).ToDictionary(x => x.EmployeeId, x => x);
                var approvalStatus = await db.TimeSheetApprovals.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.WeekId == weekId);

                var mapped = string.IsNullOrWhiteSpace(approvalStatus?.TimeApprovalStatus) ? TimeApprovalStatus.Unkown : Enum.Parse<TimeApprovalStatus>(approvalStatus.TimeApprovalStatus);
                var existing = weekDataByEmployeeId.Select(x => new TimeSummaryDTO()
                {
                    EmployeeId = x.Key,
                    Hours = x.Value.TotalRegularHours,
                    OvertimeHours = x.Value.TotalOverTimeHours,
                    WeekId = weekId,
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
                    ApprovalStatus = mapped,
                    ResponseDate = approvalStatus?.ApprovalDate,
                    SubmittedDate = approvalStatus?.SubmittedDate
                };
            }
        }
    }
}
