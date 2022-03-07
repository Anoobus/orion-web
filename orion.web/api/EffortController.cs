using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeEntries;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace orion.web.api
{
    public class ApiResult<T>
    {
        public T Data { get; set; }
        public ValidationProblemDetails Errors { get; set; }
    }

    public class NoResult
    {
        private static readonly NoResult _instance = new NoResult();
        static NoResult Instance => _instance;
    }
    public class NewEffort
    {
        public int SelectedTaskId { get; set; }
        public int SelectedJobId { get; set; }
    }

    [Route("orion-api/v1")]
    public class EffortsController : Controller
    {
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IAddNewJobTaskComboCommand _addNewJobTaskComboCommand;
        private readonly IRemoveRowCommand _removeRowCommand;
        private readonly IModifyJobTaskComboCommand _modifyJobTaskComboCommand;

        public EffortsController(ISessionAdapter sessionAdapter, IAddNewJobTaskComboCommand addNewJobTaskComboCommand, IRemoveRowCommand removeRowCommand, IModifyJobTaskComboCommand modifyJobTaskComboCommand)
        {
            _sessionAdapter = sessionAdapter;
            _addNewJobTaskComboCommand = addNewJobTaskComboCommand;
            _removeRowCommand = removeRowCommand;
            _modifyJobTaskComboCommand = modifyJobTaskComboCommand;
        }

        [HttpPost]
        [Route("employees/{employee-id}/time-entry/week/{week-id:int}/efforts")]
        public async Task<IActionResult> Index([FromBody] NewEffort effort, [FromRoute(Name = "week-id")] int weekid, [FromRoute(Name = "employee-id")] int employeeId)
        {
            var currentUserId = await _sessionAdapter.EmployeeIdAsync();
            if (!User.IsInRole(UserRoleName.Admin) && employeeId != currentUserId)
            {
                var msg = "You are not allowed to edit another users effort selection.";
                return CreateErrorResponse(msg);

            }

            var addResult = await _addNewJobTaskComboCommand.AddNewJobTaskCombo(employeeId, weekid, effort.SelectedTaskId, effort.SelectedJobId);

            if (addResult.Successful)
            {
                NotificationsController.AddNotification(User.SafeUserName(), "The selected task has been added.");
                return new StatusCodeResult(StatusCodes.Status201Created);
            }
            else
            {
                return CreateErrorResponse(string.Join(", ", addResult.Errors));
            }
        }
        [HttpDelete]
        [Route("employees/{employee-id}/time-entry/week/{week-id:int}/efforts/{job-id:int}.{task-id:int}")]
        public async Task<IActionResult> Index([FromRoute(Name = "week-id")] int weekid,
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

            var removeResult = await _removeRowCommand.RemoveRow(employeeId, weekid, taskId, jobId);

            if (removeResult.Successful)
            {
                NotificationsController.AddNotification(User.SafeUserName(), "The selected effort was removed.");
                return new ObjectResult(new ApiResult<string>()
                {
                    Data = "The selected effort was removed."
                })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                return CreateErrorResponse(string.Join(", ", removeResult.Errors));
            }
        }
        public class EffortSwitchModel
        {
            public int oldJobId { get; set; }
            public int oldTaskid {get;set;}
            public int newTaskId { get; set; }
            public int newJobId { get; set; }
        }

        [HttpPost]
        [Route("employees/{employee-id}/time-entry/week/{week-id:int}/efforts/switch")]
        public async Task<IActionResult> Index( [FromRoute(Name = "week-id")] int weekid,
            [FromRoute(Name = "employee-id")] int employeeId,
            [FromBody] EffortSwitchModel request)
        {
            var currentUserId = await _sessionAdapter.EmployeeIdAsync();
            if (!User.IsInRole(UserRoleName.Admin) && employeeId != currentUserId)
            {
                var msg = "You are not allowed to edit another users effort selection.";
                return CreateErrorResponse(msg);

            }

            var removeResult = await _modifyJobTaskComboCommand.ModifyJobTaskCombo(employeeId, weekid, request.newTaskId, request.newJobId, request.oldTaskid, request.oldJobId);
                       
            if (removeResult.Successful)
            {
                NotificationsController.AddNotification(User.SafeUserName(), "The selected effort was updated.");
                return new ObjectResult(new ApiResult<string>()
                {
                     Data = "The selected effort was updated."
                })
                {
                   StatusCode =  StatusCodes.Status200OK
                };
            }
            else
            {
                return CreateErrorResponse(string.Join(", ", removeResult.Errors));
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

