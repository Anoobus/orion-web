using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeEntries;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace orion.web.api
{

    

    public class Day
    {
        [Range(0, double.MaxValue)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)")]        
        public decimal Hours { get; set; }
        [Range(0, double.MaxValue)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)")]        
        public decimal OvertimeHours { get; set; }
    }

    public class UpdateTimeEntry : Dictionary<DayOfWeek, Day>
    {

    }

    [Route("orion-api/v1")]
    public class TimeController : Controller
    {
        private readonly ISessionAdapter _sessionAdapter;
        private readonly ISaveTimeEntriesCommand _saveTimeEntriesCommand;
        private readonly IWeekOfTimeEntriesQuery _weekOfTimeEntriesQuery;

        public TimeController(ISessionAdapter sessionAdapter, ISaveTimeEntriesCommand saveTimeEntriesCommand, IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery)
        {
            _sessionAdapter = sessionAdapter;
            _saveTimeEntriesCommand = saveTimeEntriesCommand;
            _weekOfTimeEntriesQuery = weekOfTimeEntriesQuery;
        }

        [HttpPatch]
        [Route("employees/{employee-id}/time-entry/week/{week-id:int}/efforts/{job-id:int}.{task-id:int}")]
        public async Task<IActionResult> SetTimeForEffort([FromBody] Dictionary<DayOfWeek, Day> saveRequest,
            [FromRoute(Name = "week-id")] int weekid,
            [FromRoute(Name = "employee-id")] int employeeId,
            [FromRoute(Name = "job-id")] int jobId,
            [FromRoute(Name = "task-id")] int taskId)
        {
            var currentUserId = await _sessionAdapter.EmployeeIdAsync();
            if (!User.IsInRole(UserRoleName.Admin) && employeeId != currentUserId)
            {
                var msg = "You are not allowed to edit another users effort selection.";
                return CreateErrorResponse(msg);

            }

            var currentTime = await _weekOfTimeEntriesQuery.GetFullTimeEntryViewModelAsync(new WeekOfTimeEntriesRequest()
            {
                EmployeeId = employeeId,
                RequestingUserIsAdmin = User.IsInRole(UserRoleName.Admin),
                RequestingUserName = User.SafeUserName(),
                WeekId = weekid
            });
            var rowToChange = currentTime.TimeEntryRow.FirstOrDefault(x => x.SelectedJobId == jobId & x.SelectedTaskId == taskId);
            foreach (var day in rowToChange.AllDays())
            {
                day.Hours = saveRequest[day.DayOfWeek].Hours;
                day.OvertimeHours = saveRequest[day.DayOfWeek].OvertimeHours;

            }
            
            var addResult = await _saveTimeEntriesCommand.SaveTimeEntriesAsync(0,0,null);

            if (addResult.Successful)
            {
                NotificationsController.AddNotification(User.SafeUserName(), "Time saved");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            else
            {
                return CreateErrorResponse(string.Join(", ", addResult.Errors));
            }
        }

        private static IActionResult CreateErrorResponse(string msg)
        {
            return new ObjectResult(new ApiResult<NoResult>()
            {
                Errors = new ValidationProblemDetails()
                {
                    Detail = msg,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Couldn't add new Effort",

                }
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}

