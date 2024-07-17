using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Orion.Web.BLL.Authorization;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.TimeEntries;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeApproval
{
    public interface ITimeApprovalListQuery
    {
        Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null, ActiveSection? activeSection = null);
    }

    public class TimeApprovalListQuery : ITimeApprovalListQuery, IAutoRegisterAsSingleton
    {
        private readonly ITimeSummaryService _timeSummaryService;
        private readonly ITimeApprovalService _timeApprovalService;
        private readonly IEmployeeRepository _employeeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAdapter _sessionAdapter;

        public TimeApprovalListQuery(
            ITimeSummaryService timeSummaryService,
            ITimeApprovalService timeApprovalService,
            IEmployeeRepository employeeService,
            IHttpContextAccessor httpContextAccessor,
            ISessionAdapter sessionAdapter)
        {
            _timeSummaryService = timeSummaryService;
            _timeApprovalService = timeApprovalService;
            _employeeService = employeeService;
            _httpContextAccessor = httpContextAccessor;
            _sessionAdapter = sessionAdapter;
        }

        private (IEnumerable<TimeApprovalDTO> filteredApprovalEntries, IEnumerable<EmployeeDTO> filteredEmployeeEntries) FilterToPoolOfEmployees(HashSet<int> employeesToKeep, IEnumerable<TimeApprovalDTO> approvalEntries, IEnumerable<EmployeeDTO> employees)
        {
            employees = employees.Where(x => employeesToKeep.Contains(x.EmployeeId)).ToList();

            var final = new List<TimeApprovalDTO>();
            foreach (var xcv in approvalEntries)
            {
                if (employeesToKeep.Contains( xcv.EmployeeId))
                {
                    final.Add(xcv);
                }
            } 

            return (final, employees);
        }

        public async Task<TimeApprovalList> GetApprovalListAsync(DateTime? beginDate = null, DateTime? endDate = null, ActiveSection? activeSection = null)
        {
            var thisWeek = WeekDTO.CreateWithWeekContaining(endDate ?? DateTime.Now);
            beginDate = beginDate ?? thisWeek.Previous().Previous().WeekStart;
            endDate = endDate ?? thisWeek.WeekEnd;

            var allEntries = (await _timeApprovalService.GetByStatus(
                beginDate,
                endDate,
                TimeApprovalStatus.Approved,
                TimeApprovalStatus.Rejected,
                TimeApprovalStatus.Submitted,
                TimeApprovalStatus.Unkown)).ToList();

            var allEmployees = await _employeeService.GetAllEmployees();
            allEmployees = allEmployees.Where(x => x.UserName != "admin@company.com" && x.Role != UserRoleName.Disabled).ToList();

            var pool = new HashSet<int>();
            
            if(_httpContextAccessor.HttpContext.User.CanManageTimeEntryApprovals())
            {
                pool = allEmployees.Select(x => x.EmployeeId).ToHashSet();
            }
            else if(_httpContextAccessor.HttpContext.User.CanViewOtherUsersTime())
            {
                var currentEmployeeId = await _sessionAdapter.EmployeeIdAsync();
                var viewingEmployee = allEmployees.Single(x => x.EmployeeId == currentEmployeeId);
                pool = viewingEmployee.DirectReports.ToHashSet();
            }

            var (entries, employees) = FilterToPoolOfEmployees(pool, allEntries, allEmployees);

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
                foreach (var emp in employees)
                {
                    if (!entries.Any(x => x.EmployeeId == emp.EmployeeId && x.WeekId == wk.WeekId.Value && x.TimeApprovalStatus != TimeApprovalStatus.Unkown))
                    {
                        missing.Add((emp, wk));
                    }
                }
            }

            var allMissing = (from p in missing
                              join c in entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Unkown).ToList()
                                  on new { p.emp.EmployeeId, WeekId = p.week.WeekId.Value } equals new { c.EmployeeId, c.WeekId } into ps
                              from x in ps.DefaultIfEmpty()
                              select
                                  new TimeApprovalDTO()
                                  {
                                      EmployeeId = p.emp.EmployeeId,
                                      EmployeeName = $"{p.emp.Last}, {p.emp.First}",
                                      TimeApprovalStatus = TimeApprovalStatus.Unkown,
                                      WeekId = p.week.WeekId.Value,
                                      WeekStartDate = p.week.WeekStart,
                                      TotalOverTimeHours = x == null ? 0.00m : x.TotalOverTimeHours,
                                      TotalRegularHours = x == null ? 0.00m : x.TotalRegularHours,
                                      IsHidden = x == null ? false : x.IsHidden
                                  }).ToList();

            var currentUserCanManageTimeApprovals = _httpContextAccessor.HttpContext.User.CanManageTimeEntryApprovals();

            return new TimeApprovalList()
            {
                PeriodEndDate = endDate.Value,
                PeriodStartData = beginDate.Value,
                ApprovedEntries = new TimeEntriesViewModel() { Entries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Approved && !x.IsHidden), CurrentUserCanManageTimeApprovals = currentUserCanManageTimeApprovals },
                RejectedEntries = new TimeEntriesViewModel() { Entries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Rejected && !x.IsHidden), CurrentUserCanManageTimeApprovals = currentUserCanManageTimeApprovals },
                SubmittedEntries = new TimeEntriesViewModel() { Entries = entries.Where(x => x.TimeApprovalStatus == TimeApprovalStatus.Submitted && !x.IsHidden), CurrentUserCanManageTimeApprovals = currentUserCanManageTimeApprovals },
                MissingEntries = new TimeEntriesViewModel() { Entries = allMissing.Where(x => !x.IsHidden).OrderBy(x => x.WeekId).ToList(), CurrentUserCanManageTimeApprovals = currentUserCanManageTimeApprovals },
                HiddenEntries = new TimeEntriesViewModel() { Entries = entries.Where(x => x.IsHidden).ToList(), CurrentUserCanManageTimeApprovals = currentUserCanManageTimeApprovals },
                ActiveSection = activeSection ?? ActiveSection.Submitted
            };
        }
    }
}
