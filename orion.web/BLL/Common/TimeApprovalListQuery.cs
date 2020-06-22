using orion.web.Employees;
using orion.web.TimeApproval;
using orion.web.TimeEntries;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface ITimeApprovalListQuery
    {
        Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null);
    }
    public class TimeApprovalListQuery : ITimeApprovalListQuery, IAutoRegisterAsSingleton
    {
        private readonly ITimeSummaryService timeSummaryService;
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IEmployeeRepository employeeService;

        public TimeApprovalListQuery(
            ITimeSummaryService timeSummaryService, ITimeApprovalService timeApprovalService,
            IEmployeeRepository employeeService)
        {
            this.timeSummaryService = timeSummaryService;
            this.timeApprovalService = timeApprovalService;
            this.employeeService = employeeService;
        }
        public async Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null)
        {
            var thisWeek = WeekDTO.CreateWithWeekContaining(endDate ?? DateTime.Now);
            beginDate = beginDate ?? thisWeek.Previous().WeekStart;
            endDate = endDate ?? thisWeek.WeekEnd;
            var entries = (await timeApprovalService.GetByStatus(beginDate, endDate,
                TimeApprovalStatus.Approved,
                TimeApprovalStatus.Rejected,
                TimeApprovalStatus.Submitted,
                TimeApprovalStatus.Unkown)).ToList();

            var allEmployees = await employeeService.GetAllEmployees();
            allEmployees = allEmployees.Where(x => x.UserName != "admin@company.com").ToList();
            var next = WeekDTO.CreateWithWeekContaining(beginDate.Value);
            var end = WeekDTO.CreateWithWeekContaining(endDate.Value);
            var allWeeks = new List<WeekDTO>() { end };
            while (next != end)
            {
                allWeeks.Add(next);
                next = next.Next();
            }

            var missing = new List<(EmployeeDTO emp, WeekDTO week)>();
            foreach (var wk in allWeeks)
            {
                foreach (var emp in allEmployees)
                {
                    if(!entries.Any(x => x.EmployeeId == emp.EmployeeId && x.WeekId == wk.WeekId.Value && x.TimeApprovalStatus != TimeApprovalStatus.Unkown))
                    {
                        missing.Add((emp, wk));
                    }
                }
            }
            var allMissing =    (from p in missing
                                join c in entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Unkown).ToList()
                                    on new { p.emp.EmployeeId, WeekId = p.week.WeekId.Value } equals new { c.EmployeeId, c.WeekId } into ps
                                from x in ps.DefaultIfEmpty()
                                select
                                    new TimeApprovalDTO()
                                    {
                                        EmployeeId =  p.emp.EmployeeId,
                                        EmployeeName = $"{p.emp.Last}, {p.emp.First}",
                                        TimeApprovalStatus = TimeApprovalStatus.Unkown,
                                        WeekId = p.week.WeekId.Value,
                                        WeekStartDate = p.week.WeekStart,
                                        TotalOverTimeHours = x == null ? 0 : x.TotalOverTimeHours,
                                        TotalRegularHours = x == null ? 0 : x.TotalRegularHours,
                                        IsHidden = x == null ? false : x.IsHidden
                                    }).ToList();

            return new TimeApprovalList()
            {
                PeriodEndDate = endDate.Value,
                PeriodStartData = beginDate.Value,
                ApprovedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Approved && !x.IsHidden),
                RejectedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Rejected && !x.IsHidden),
                SubmittedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Submitted && !x.IsHidden),
                MissingEntries = allMissing.Where(x => !x.IsHidden).OrderBy(x => x.WeekId).ToList(),
                HiddenEntries = entries.Where(x => x.IsHidden).ToList()
            };

        }
    }
}
