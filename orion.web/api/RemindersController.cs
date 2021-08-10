using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.DataAccess.EF;
using orion.web.Employees;
using orion.web.TimeEntries;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    [Authorize]
    [Route("api/v1/reminder")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        public const string PPE_MISSING_TIME_TASK = "on-ppe-missing-time-email";
        public const string NONPPE_MISSING_TIME_TASK = "off-ppe-missing-time-email";

        private readonly ITimeApprovalService _timeApprovalService;
        private readonly ISmtpProxy _smtpProxy;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IScheduledTaskRepo _scheduleTaskRepo;
        private readonly ILogger _logger = Log.ForContext< RemindersController>();

        public RemindersController(ITimeApprovalService timeApprovalService, ISmtpProxy smtpProxy, IEmployeeRepository employeeRepository, IScheduledTaskRepo scheduleTaskRepo, ILogger logger)
        {
            _timeApprovalService = timeApprovalService;
            _smtpProxy = smtpProxy;
            _employeeRepository = employeeRepository;
            _scheduleTaskRepo = scheduleTaskRepo;
            _logger = logger;
        }

        [HttpPost("on-ppe-missing-time-email")]
        public async Task<ActionResult> SendUnsubmittedTimeReminderPPE()
        {
            var reminderTask = await _scheduleTaskRepo.GetTaskByName(PPE_MISSING_TIME_TASK);
            reminderTask = reminderTask ?? (await CreateReminderTask(PPE_MISSING_TIME_TASK, isPpeOnly: true));
            _logger.Information($"Checking if {reminderTask} should run");
            if(await ShouldRun(reminderTask))
            {
                _logger.Information($"{reminderTask} now running");
                await _scheduleTaskRepo.RecordTaskCompletion(reminderTask.ScheduleTaskId);
                await SendReminders();
            }

            return Ok();
        }

        private async Task<bool> ShouldRun(ScheduleTask reminderTask)
        {
            var rightNow = DateTimeOffset.Now;
            var isAfterStartDate = rightNow.Subtract(reminderTask.StartDate).TotalSeconds > 0;
            var isCorrectDayOfWeek = MatchesDay(reminderTask, rightNow);

            return isAfterStartDate
                && isCorrectDayOfWeek
                && (await MeetsRecurrenceCriteria(reminderTask, rightNow));
        }

        private async Task<bool> MeetsRecurrenceCriteria(ScheduleTask reminderTask, DateTimeOffset now)
        {
            var lastRun = await _scheduleTaskRepo.GetLastRunTime(reminderTask.ScheduleTaskId);
            var taskHasNeverRan = !lastRun.HasValue;
            var hasNotRanWithin_N_Weeks = !taskHasNeverRan && now.Subtract(lastRun.Value).TotalDays > (reminderTask.RecurEveryNWeeks * 7);
            return taskHasNeverRan || hasNotRanWithin_N_Weeks;
        }
        private async Task SendReminders()
        {
            var thisWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            if(thisWeek.IsPPE.Value)
            {
                var beginDate = thisWeek.Previous().WeekStart;
                var endDate = thisWeek.WeekEnd;
                var entries = (await _timeApprovalService.GetByStatus(beginDate, endDate, TimeApprovalStatus.Unkown)).ToList();
                var template = await System.IO.File.ReadAllTextAsync(@"wwwroot\email-submit-time-reminder.html");
                foreach(var entry in entries.GroupBy(x => x.EmployeeId))
                {
                    try
                    {
                        var emp = await _employeeRepository.GetSingleEmployeeAsync(entry.Key);
                        _logger.Information($"sending reminder to {emp.First} {emp.Last} [{emp.UserName}]");

                        _smtpProxy.SendMail(emp.UserName, template, $"Reminder to submit time for {beginDate.ToShortDateString()}-{endDate.ToShortDateString()}");
                    }
                    catch(Exception  e)
                    {
                        _logger.Error(e, "error trying to send reminder");
                    }

                }
            }
        }

        private Dictionary<DayOfWeek, Func<ScheduleTask, bool>> _dayMap = new Dictionary<DayOfWeek, Func<ScheduleTask, bool>>()
        {
            {DayOfWeek.Monday,  x=> x.OnMonday  },
            {DayOfWeek.Tuesday,  x=> x.OnTuesday  },
            {DayOfWeek.Wednesday,  x=> x.OnWednesday  },
            {DayOfWeek.Thursday,  x=> x.OnThursday  },
            {DayOfWeek.Friday,  x=> x.OnFriday  },
            {DayOfWeek.Saturday,  x=> x.OnSaturday  },
            {DayOfWeek.Sunday,  x=> x.OnSunday  }
        };

        private bool MatchesDay(ScheduleTask task, DateTimeOffset dateTimeOffset)
        {

            var dayOfWeek = dateTimeOffset.ToLocalTime().DayOfWeek;
            var expr = _dayMap[dayOfWeek];
            return expr(task);
        }

        private async Task<DataAccess.EF.ScheduleTask> CreateReminderTask(string taskName, bool isPpeOnly)
        {
            var thisWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            if(isPpeOnly)
            {
                if(!thisWeek.IsPPE.Value)
                    thisWeek = thisWeek.Previous();
            }
            _logger.Information($"Creating new Reminder Task [{taskName}]");
            return await _scheduleTaskRepo.CreateNewScheduledTask(new BLL.ScheduledTasks.NewScheduledTask()
            {
                EndDate = null,
                OnFriday = true,
                RecurEveryNWeeks = isPpeOnly ? 2 : 1,
                StartDate = thisWeek.WeekStart,
                TaskName = taskName
            });
        }


        //[HttpPost("off-ppe-missing-time-email")]
        //public async Task<ActionResult> SendUnsubmittedTimeReminder()
        //{
        //    var thisWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now);

        //    var reminderTask = await _scheduleTaskRepo.GetTaskByName(NONPPE_MISSING_TIME_TASK);
        //    reminderTask = reminderTask ?? (await CreateReminderTask(NONPPE_MISSING_TIME_TASK, isPpeOnly: false));

        //    if(await ShouldRun(reminderTask))
        //    {
        //        var entries = (await _timeApprovalService.GetByStatus(thisWeek.WeekStart, thisWeek.WeekEnd, TimeApprovalStatus.Unkown)).ToList();
        //        var template = await System.IO.File.ReadAllTextAsync(@"wwwroot\email-submit-time-reminder.html");
        //        foreach(var entry in entries)
        //        {
        //            var emp = await _employeeRepository.GetSingleEmployeeAsync(entry.EmployeeId);
        //            _smtpProxy.SendMail(emp.UserName, template, $"Quick reminder to submit time for week {thisWeek.WeekStart.ToShortDateString()}-{thisWeek.WeekEnd.ToShortDateString()}");
        //        }
        //    }

        //    return Ok();
        //}


    }
}
