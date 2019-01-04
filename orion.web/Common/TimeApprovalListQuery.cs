using orion.web.Employees;
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
        //private readonly IWeekService weekService;
        private readonly ITimeSummaryService timeSummaryService;
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IEmployeeService employeeService;

        public TimeApprovalListQuery(
            //IWeekService weekService, 
            ITimeSummaryService timeSummaryService, ITimeApprovalService timeApprovalService,
            IEmployeeService employeeService)
        {
            //this.weekService = weekService;
            this.timeSummaryService = timeSummaryService;
            this.timeApprovalService = timeApprovalService;
            this.employeeService = employeeService;
        }
        public async Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null)
        {
            beginDate = beginDate ?? DateTime.Now.AddDays(-60);
            endDate = endDate ?? DateTime.Now;
            var entries = (await timeApprovalService.GetByStatus(beginDate, endDate,
                TimeApprovalStatus.Approved,
                TimeApprovalStatus.Rejected,
                TimeApprovalStatus.Submitted)).ToList();

            var allEmployees = await employeeService.GetAllEmployees();

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
                    if(!entries.Any(x => x.EmployeeId == emp.EmployeeId && x.WeekId == wk.WeekId.Value))
                    {
                        missing.Add((emp, wk));
                    }
                }
            }

            var allMissing = missing.Select(x => new TimeApprovalDTO()
            {
                EmployeeId = x.emp.EmployeeId,
                EmployeeName = x.emp.UserName,
                TimeApprovalStatus = TimeApprovalStatus.Unkown,
                WeekId = x.week.WeekId.Value,
                WeekStartDate = x.week.WeekStart,
            });

            return new TimeApprovalList()
            {
                PeriodEndDate = endDate.Value,
                PeriodStartData = beginDate.Value,
                ApprovedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Approved),
                RejectedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Rejected),
                SubmittedEntries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Submitted),
                MissingEntries = allMissing.OrderBy(x => x.WeekId).ToList()
            };

        }
    }
}
