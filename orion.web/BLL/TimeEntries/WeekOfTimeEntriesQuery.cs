using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.JobsTasks;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
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
        private readonly ITimeService timeService;
        private readonly IJobService jobService;
        private readonly ITaskService taskService;
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IEmployeeRepository employeeService;
        private readonly IExpenseService _expenseService;

        public WeekOfTimeEntriesQuery(ITimeService timeService,
            IJobService jobService,
            ITaskService taskService,
            ITimeApprovalService timeApprovalService,
            IEmployeeRepository employeeService,
            IExpenseService expenseService
            )
        {
            this.timeService = timeService;
            this.jobService = jobService;
            this.taskService = taskService;
            this.timeApprovalService = timeApprovalService;
            this.employeeService = employeeService;
            _expenseService = expenseService;
        }

        public async Task<FullTimeEntryViewModel> GetFullTimeEntryViewModelAsync(WeekOfTimeEntriesRequest request)
        {

            var employeeForWeek = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
            if(!request.RequestingUserIsAdmin && employeeForWeek.UserName != request.RequestingUserName)
            {
                throw new UnauthorizedAccessException($"You are not allowed to view others time sheets");
            }

            var currentWeek = WeekDTO.CreateForWeekId(request.WeekId);

            var timeEntries = await timeService.GetAsync(request.WeekId, request.EmployeeId);
            var allJobList = (await jobService.GetAsync()).ToList();
            var taskList = (await taskService.GetTasks()).ToList().OrderBy(x => x.Name).ThenBy(x => x.Description);
            var entries = new List<TimeEntryViewModel>();
            var status = await timeApprovalService.GetAsync(request.WeekId, request.EmployeeId);

            foreach(var entry in timeEntries.GroupBy(x => new { x.JobId, x.JobTaskId }))
            {
                var tasky = taskList.FirstOrDefault(x => x.TaskId == entry.Key.JobTaskId);
                var SelectedEntryTaskName = $"{tasky?.LegacyCode} - {tasky?.Name}";

                var SelectedEntryJob = allJobList.FirstOrDefault(x => x.JobId == entry.Key.JobId);
                var jobCode = "";
                if(SelectedEntryJob != null)
                {
                    jobCode = SelectedEntryJob.JobCode;
                }
                var item = new TimeEntryViewModel()
                {
                    SelectedJobId = entry.Key.JobId,
                    SelectedTaskId = entry.Key.JobTaskId,
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

            entries = entries.OrderBy(x => x.SelectedJobCode).ToList();
            var nextWeek = currentWeek.Next();
            var prevWeek = currentWeek.Previous();

            return new FullTimeEntryViewModel()
            {
                TimeEntryRow = entries,
                EmployeeId = employeeForWeek.EmployeeId,
                NewEntry = await GenerateEmptyJobTaskAsync(request.WeekId, request.EmployeeId),
                Week = new WeekIdentifier()
                {
                    WeekEnd = currentWeek.WeekEnd,
                    WeekStart = currentWeek.WeekStart,
                    WeekId = currentWeek.WeekId.Value,
                    Year = currentWeek.Year
                },
                ApprovalStatus = status.TimeApprovalStatus,

                Expenses = await _expenseService.GetExpensesForEmployee(request.EmployeeId,request.WeekId)
            };


        }

        private async Task<NewJobTaskCombo> GenerateEmptyJobTaskAsync(int weekId, int employeeId)
        {
            var jobList = (await jobService.GetAsync(employeeId)).ToList();
            var taskList = (await taskService.GetTasks()).Where(x => x.UsageStatus.Enum == JobTasks.UsageStatus.Enabled).ToList().OrderBy(x => x.LegacyCode).ThenBy(x => x.Name);
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
                Date = GetDateFor(week, dayOfWeek),
                Hours = 0
            };
        }

        private DateTime GetDateFor(WeekDTO week, DayOfWeek dayOfWeek)
        {

            var candidateDay = week.WeekStart;
            while(candidateDay.DayOfWeek != dayOfWeek)
            {
                candidateDay = candidateDay.AddDays(1);
            }
            return candidateDay;
        }
        private TimeSpentViewModel MapToViewModel(WeekDTO week, DayOfWeek dayOfWeek, IEnumerable<TimeEntryDTO> allEntriesThisWeek)
        {
            var thisDaysEntry = allEntriesThisWeek.SingleOrDefault(x => x.Date.DayOfWeek == dayOfWeek);
            var date = GetDateFor(week, dayOfWeek);
            return new TimeSpentViewModel()
            {
                DayOfWeek = dayOfWeek,
                Date = date,
                Hours = thisDaysEntry?.Hours ?? 0,
                OvertimeHours = thisDaysEntry?.OvertimeHours ?? 0,
                TimeEntryId = thisDaysEntry?.TimeEntryId ?? 0
            };
        }

    }
}
