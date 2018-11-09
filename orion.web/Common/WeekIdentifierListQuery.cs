using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface IWeekIdentifierListQuery : IRegisterByConvention
    {
        Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, string employeeName);
    }
    public class WeekIdentifierListQuery : IWeekIdentifierListQuery
    {
        private readonly IWeekService weekService;
        private readonly ITimeSummaryService timeSummaryService;

        public WeekIdentifierListQuery(IWeekService weekService, ITimeSummaryService timeSummaryService)
        {
            this.weekService = weekService;
            this.timeSummaryService = timeSummaryService;
        }
        public async Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, string employeeName)
        {
            var dt = DateTime.Now;
            var thisWeek = weekService.Get(DateTime.Now);
            var weeks = new List<DetailedWeekIdentifier>();
            while(entriesToShow-- > 0)
            {
                var temp = weekService.Previous(thisWeek.Year, thisWeek.WeekId);
                var thisWeekTimeSummary = await timeSummaryService.GetAsync(temp.Year, temp.WeekId, employeeName);
                weeks.Add(new DetailedWeekIdentifier()
                {
                    WeekEnd = weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_END),
                    WeekStart = weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_START),
                    WeekId = temp.WeekId,
                    Year = temp.Year,
                    ApprovalStatus = thisWeekTimeSummary.ApprovalStatus,
                    TotalOverTime = thisWeekTimeSummary.OvertimeHours,
                    TotalRegular = thisWeekTimeSummary.Hours
                });
                thisWeek = temp;
            }
            return new WeekListViewModel()
            {
                Weeks = weeks
            };
        }
    }
}
