using orion.web.TimeApproval;
using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface ITimeApprovalListQuery : IRegisterByConvention
    {
        Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null);
    }
    public class TimeApprovalListQuery : ITimeApprovalListQuery
    {
        private readonly IWeekService weekService;
        private readonly ITimeSummaryService timeSummaryService;
        private readonly ITimeApprovalService timeApprovalService;

        public TimeApprovalListQuery(IWeekService weekService, ITimeSummaryService timeSummaryService, ITimeApprovalService timeApprovalService)
        {
            this.weekService = weekService;
            this.timeSummaryService = timeSummaryService;
            this.timeApprovalService = timeApprovalService;
        }
        public async Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null)
        {
            beginDate = beginDate ?? DateTime.Now.AddDays(-60);
            endDate = endDate ?? DateTime.Now;
            var entries = (await timeApprovalService.GetByStatus(beginDate, endDate,
                TimeApprovalStatus.Approved,
                TimeApprovalStatus.Rejected,
                TimeApprovalStatus.Submitted)).ToList();
            return new TimeApprovalList()
            {
                PeriodEndDate = endDate.Value,
                PeriodStartData = beginDate.Value,
                ApprovedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Approved),
                RejectedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Rejected),
                SubmittedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Submitted)
            };
           
        }
    }
}
