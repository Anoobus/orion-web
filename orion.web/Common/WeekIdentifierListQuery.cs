using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface IWeekIdentifierListQuery : IRegisterByConvention
    {
        Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, int employeeId, DateTime? startDate = null);
    }
    public class WeekIdentifierListQuery : IWeekIdentifierListQuery
    {
        //private readonly IWeekService weekService;
        private readonly ITimeSummaryService timeSummaryService;

        public WeekIdentifierListQuery(//IWeekService weekService, 
            ITimeSummaryService timeSummaryService)
        {
            //this.weekService = weekService;
            this.timeSummaryService = timeSummaryService;
        }
        public async Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, int employeeId, DateTime? startDate = null)
        {
            var dt = DateTime.Now;
            var thisWeek = WeekDTO.CreateWithWeekContaining(startDate ?? DateTime.Now);
            var currWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekId.Value;
            var weeks = new List<DetailedWeekIdentifier>();
            while(entriesToShow-- > 0)
            {
                var temp = thisWeek;
                var thisWeekTimeSummary = await timeSummaryService.GetAsync(temp.Year, temp.WeekId.Value, employeeId);
                weeks.Add(new DetailedWeekIdentifier()
                {
                    WeekEnd = thisWeek.WeekEnd,//weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_END),
                    WeekStart = thisWeek.WeekStart,//weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_START),
                    WeekId = temp.WeekId.Value,
                    Year = temp.Year,
                    ApprovalStatus = thisWeekTimeSummary.ApprovalStatus,
                    TotalOverTime = thisWeekTimeSummary.OvertimeHours,
                    TotalRegular = thisWeekTimeSummary.Hours,
                     IsCurrentWeek = currWeek == temp.WeekId.Value
                });
                thisWeek = thisWeek.Previous();// weekService.Previous(thisWeek.Year, thisWeek.WeekId); 
            }
            return new WeekListViewModel()
            {
                Weeks = weeks,
                EmployeeId = employeeId,
                WeeksToShow = entriesToShow,
                StartWithDate = startDate ?? DateTime.Now
            };
        }
    }
}
