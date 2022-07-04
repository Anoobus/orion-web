using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeApproval;
using orion.web.TimeEntries;

namespace orion.web.api
{
    public class TimeApprovalUpdateRequest
    {
        public TimeApprovalStatus NewApprovalState { get; set; }
    }

    [Route("orion-api/v1")]
    [ApiController]
    public class TimeApprovalController : Controller
    {
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IApproveTimeCommand _approveTimeCommand;
        private readonly ITimeApprovalService _timeApprovalService;

        public TimeApprovalController(ISessionAdapter sessionAdapter, IApproveTimeCommand approveTimeCommand, ITimeApprovalService timeApprovalService)
        {
            _sessionAdapter = sessionAdapter;
            _approveTimeCommand = approveTimeCommand;
            _timeApprovalService = timeApprovalService;
        }

        [HttpPut]
        [Route("time-approval/employees/{employee-id}/week/{week-id:int}")]
        public async Task<IActionResult> SetTimeForEffort([FromBody] TimeApprovalUpdateRequest saveRequest,
            [FromRoute(Name = "employee-id")] int employeeId,
            [FromRoute(Name = "week-id")] int weekId)
        {
            try
            {
                var currentUser = await _sessionAdapter.EmployeeIdAsync();
                var isAdmin = this.HttpContext.User.IsInRole(UserRoleName.Admin);
                var request = new TimeApprovalRequest(
                    approvingUserId: currentUser,
                    approvingUserIsAdmin: isAdmin,
                    employeeId: employeeId,
                    newApprovalState: saveRequest.NewApprovalState,
                    weekId: weekId
                );
                var rez = await _approveTimeCommand.ApplyApproval(request);

                if (rez.Successful)
                {
                    var current = await _timeApprovalService.GetAsync(request.WeekId, request.EmployeeId);                    
                    NotificationsController.AddNotification(User.SafeUserName(), $"Time approved for {current.EmployeeName}");
                    return new OkObjectResult(new { SubmittedDate = current.SubmittedDate.Value.ToShortDateString(), current.TimeApprovalStatus });
                }
                else
                {
                    return CreateErrorResponse(string.Join(", ", rez.Errors));
                }
            }
            catch (Exception ex)
            {
                var hmm = ex;
                throw;
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
                    Title = "Couldn't approve time.",

                }
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}

