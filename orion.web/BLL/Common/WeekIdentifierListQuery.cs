using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Web.Employees;
using Orion.Web.TimeEntries;
using Orion.Web.Util.IoC;

namespace Orion.Web.Common
{
    public interface IWeekIdentifierListQuery
    {
        Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, int employeeId, DateTime? startDate = null);
    }

    public class WeekIdentifierListQuery : IWeekIdentifierListQuery, IAutoRegisterAsSingleton
    {
        private readonly ITimeSummaryService timeSummaryService;
        private readonly IEmployeeRepository _employeeRepository;

        public WeekIdentifierListQuery(
            ITimeSummaryService timeSummaryService,
            IEmployeeRepository employeeRepository)
        {
            this.timeSummaryService = timeSummaryService;
            _employeeRepository = employeeRepository;
        }

        public async Task<WeekListViewModel> GetWeeksAsync(int entriesToShow, int employeeId, DateTime? startDate = null)
        {
            var dt = DateTime.Now;
            var thisWeek = WeekDTO.CreateWithWeekContaining(startDate ?? DateTime.Now);
            var currWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now).WeekId.Value;
            var weeks = new List<DetailedWeekIdentifier>();
            while (entriesToShow-- > 0)
            {
                var temp = thisWeek;
                var thisWeekTimeSummary = await timeSummaryService.GetAsync(temp.WeekId.Value, employeeId);
                weeks.Add(new DetailedWeekIdentifier()
                {
                    WeekEnd = thisWeek.WeekEnd, // weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_END),
                    WeekStart = thisWeek.WeekStart, // weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_START),
                    WeekId = temp.WeekId.Value,
                    Year = temp.Year,
                    ApprovalStatus = thisWeekTimeSummary.ApprovalStatus,
                    TotalOverTime = thisWeekTimeSummary.OvertimeHours,
                    TotalRegular = thisWeekTimeSummary.Hours,
                    IsCurrentWeek = currWeek == temp.WeekId.Value
                });
                thisWeek = thisWeek.Previous(); // weekService.Previous(thisWeek.Year, thisWeek.WeekId);
            }

            var user = await _employeeRepository.GetSingleEmployeeAsync(employeeId);
            var name = string.Empty;
            if (user != null)
            {
                name = $"{user.First} {user.Last}";
            }

            return new WeekListViewModel()
            {
                Weeks = weeks,
                EmployeeId = employeeId,
                EmployeeDisplayName = name,
                WeeksToShow = entriesToShow,
                StartWithDate = startDate ?? DateTime.Now
            };
        }
    }
}
