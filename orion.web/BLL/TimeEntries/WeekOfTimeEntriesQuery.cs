using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orion.Web.BLL.Authorization;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.JobsTasks;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeEntries
{
    public class WeekOfTimeEntriesRequest
    {
        public int EmployeeId { get; set; }
        public int WeekId { get; set; }
        public string RequestingUserName { get; set; }
        public bool RequestingUserIsAdmin { get; set; }
    }

    public interface IWeekOfTimeEntriesQuery
    {
        Task<FullTimeEntryViewModel> GetFullTimeEntryViewModelAsync(WeekOfTimeEntriesRequest request);
    }

    public class WeekOfTimeEntriesQuery : IWeekOfTimeEntriesQuery, IAutoRegisterAsSingleton
    {
        private readonly ITimeService _timeService;
        private readonly IJobsRepository _jobService;
        private readonly ITaskService _taskService;
        private readonly ITimeApprovalService _timeApprovalService;
        private readonly IEmployeeRepository _employeeService;
        private readonly IExpenseService _expenseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAdapter _sessionAdapter;

        public WeekOfTimeEntriesQuery(
            ITimeService timeService,
            IJobsRepository jobService,
            ITaskService taskService,
            ITimeApprovalService timeApprovalService,
            IEmployeeRepository employeeService,
            IExpenseService expenseService,
            IHttpContextAccessor httpContextAccessor,
            ISessionAdapter sessionAdapter
            )
        {
            _timeService = timeService;
            _jobService = jobService;
            _taskService = taskService;
            _timeApprovalService = timeApprovalService;
            _employeeService = employeeService;
            _expenseService = expenseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionAdapter = sessionAdapter;
        }

        public async Task<FullTimeEntryViewModel> GetFullTimeEntryViewModelAsync(WeekOfTimeEntriesRequest request)
        {
            var employeeForWeek = await _employeeService.GetSingleEmployeeAsync(request.EmployeeId);
            if (!_httpContextAccessor.HttpContext.User.CanViewOtherUsersTime() && employeeForWeek.UserName != request.RequestingUserName)
            {
                throw new UnauthorizedAccessException($"You are not allowed to view others time sheets");
            }

            var currentWeek = WeekDTO.CreateForWeekId(request.WeekId);

            var weekOfTimeEntries = await _timeService.GetWeekAsync(request.EmployeeId, request.WeekId);

            var allJobList = (await _jobService.GetAsync()).ToList();
            var taskList = (await _taskService.GetTasks())
                                              .ToList()
                                              .OrderBy(x => x.Name)
                                              .ThenBy(x => x.Description);

            var entries = new List<TimeEntryViewModel>();
            var status = await _timeApprovalService.GetAsync(request.WeekId, request.EmployeeId);

            var existingJobWithTasks = weekOfTimeEntries.AsEnumerable()
                                                        .GroupBy(x => new JobWithTaskDTO() { JobId = x.JobId, TaskId = x.JobTaskId });

            foreach (var entry in existingJobWithTasks)
            {
                var tasky = taskList.FirstOrDefault(x => x.TaskId == entry.Key.TaskId);
                var SelectedEntryTaskName = $"{tasky?.LegacyCode} - {tasky?.Name}";

                var SelectedEntryJob = allJobList.FirstOrDefault(x => x.JobId == entry.Key.JobId);
                var jobCode = "";
                if (SelectedEntryJob != null)
                {
                    jobCode = SelectedEntryJob.JobCode;
                }

                var item = new TimeEntryViewModel()
                {
                    SelectedJobId = entry.Key.JobId,
                    SelectedTaskId = entry.Key.TaskId,
                    SelectedEntryTaskName = SelectedEntryTaskName,
                    SelectedEntryJobName = SelectedEntryJob?.JobName,
                    SelectedJobCode = jobCode,
                    SelectedTaskCategory = tasky?.Category?.Name,
                    Monday = MapToViewModel(currentWeek, DayOfWeek.Monday, entry),
                    Tuesday = MapToViewModel(currentWeek, DayOfWeek.Tuesday, entry),
                    Wednesday = MapToViewModel(currentWeek, DayOfWeek.Wednesday, entry),
                    Thursday = MapToViewModel(currentWeek, DayOfWeek.Thursday, entry),
                    Friday = MapToViewModel(currentWeek, DayOfWeek.Friday, entry),
                    Saturday = MapToViewModel(currentWeek, DayOfWeek.Saturday, entry),
                    Sunday = MapToViewModel(currentWeek, DayOfWeek.Sunday, entry),
                };
                entries.Add(item);
            }

            entries = entries.OrderBy(x => x.SelectedJobCode).ThenBy(x => x.SelectedTaskId).ToList();
            var nextWeek = currentWeek.Next();
            var prevWeek = currentWeek.Previous();

            return new FullTimeEntryViewModel()
            {
                TimeEntryRow = entries,
                EmployeeId = employeeForWeek.EmployeeId,
                EmployeeDisplayName = $"{employeeForWeek.First} {employeeForWeek.Last}",
                NewEntry = await GenerateEmptyJobTaskAsync(request.WeekId, request.EmployeeId),
                Week = new WeekIdentifier()
                {
                    WeekEnd = currentWeek.WeekEnd,
                    WeekStart = currentWeek.WeekStart,
                    WeekId = currentWeek.WeekId.Value,
                    Year = currentWeek.Year
                },
                ApprovalStatus = status.TimeApprovalStatus,

                Expenses = await _expenseService.GetExpensesForEmployee(request.EmployeeId, request.WeekId),
                IncludeRowsWithNoEffortAppliedOnCopyPreviousWeekTasks = true,
                CurrentUserCanManageApprovalsForWeek = _httpContextAccessor.HttpContext.User.CanManageTimeEntryApprovals(),
                CurrentUserCanSaveOrSubmitWeek = _httpContextAccessor.HttpContext.User.CanManageTimeEntryApprovals()
                                                  || (await _sessionAdapter.EmployeeIdAsync()) == employeeForWeek.EmployeeId
            };
        }

        private async Task<NewJobTaskCombo> GenerateEmptyJobTaskAsync(int weekId, int employeeId)
        {
            var jobList = (await _jobService.GetAsync(employeeId)).ToList();
            var taskList = (await _taskService.GetTasks()).Where(x => x.UsageStatus.Enum == JobTasks.UsageStatus.Enabled).ToList().OrderBy(x => x.LegacyCode).ThenBy(x => x.Name);
            var week = WeekDTO.CreateForWeekId(weekId);
            return new NewJobTaskCombo()
            {
                SelectedJobId = null,
                SelectedTaskId = null,
                AvailableJobs = jobList,
                AvailableTasks = taskList,
            };
        }

        public TimeSpentViewModel EmptyViewModel(DayOfWeek dayOfWeek, WeekDTO week)
        {
            return new TimeSpentViewModel()
            {
                Date = week.GetDateFor(dayOfWeek),
                Hours = 0
            };
        }

        private TimeSpentViewModel MapToViewModel(WeekDTO week, DayOfWeek dayOfWeek, IEnumerable<TimeEntryBaseDTO> allEntriesThisWeek)
        {
            var thisDaysEntry = allEntriesThisWeek.SingleOrDefault(x => x.Date.DayOfWeek == dayOfWeek);
            var date = week.GetDateFor(dayOfWeek);
            return new TimeSpentViewModel()
            {
                DayOfWeek = dayOfWeek,
                Date = date,
                Hours = thisDaysEntry?.Hours ?? 0,
                OvertimeHours = thisDaysEntry?.OvertimeHours ?? 0,
            };
        }
    }
}