using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeApproval;
using System;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{

    [Authorize]
    public class TimeApprovalController : Controller
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IApproveTimeCommand approveTimeCommand;
        private readonly IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery;
        private readonly ITimeApprovalListQuery timeApprovalListQuery;
        private readonly ISessionAdapter sessionAdapter;

        public TimeApprovalController(ITimeApprovalService timeApprovalService, 
            IApproveTimeCommand approveTimeCommand,
            IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery,
            ITimeApprovalListQuery timeApprovalListQuery,
            ISessionAdapter sessionAdapter)
        {
            this.timeApprovalService = timeApprovalService;
            this.approveTimeCommand = approveTimeCommand;
            this.weekOfTimeEntriesQuery = weekOfTimeEntriesQuery;
            this.timeApprovalListQuery = timeApprovalListQuery;
            this.sessionAdapter = sessionAdapter;
        }

        public const string APPROVAL_ROUTE = "APPROVAL_ROUTE";

        public class TimeApprovalModel
        {
            public int WeekId { get; set; }
            public int EmployeeId { get; set; }
            public TimeApprovalStatus NewApprovalState { get; set; }
        }

        public async Task<ActionResult> Index()
        {
            var vm = await timeApprovalListQuery.GetApprovalListAsync();
            return View("List", vm);
        }

        [HttpPost]
        public async Task<ActionResult> Index(DateTime PeriodStartData, DateTime PeriodEndDate)
        {
            var vm = await timeApprovalListQuery.GetApprovalListAsync(beginDate: PeriodStartData, endDate: PeriodEndDate);
            return View("List", vm);
        }

        
    [HttpPost]
        [Route("TimeApproval/{newApprovalState}", Name = APPROVAL_ROUTE)]
        public async Task<ActionResult> ApplyApproval(TimeApprovalModel request)
        {
            var req = new TimeApprovalRequest(            
                approvingUserId : await sessionAdapter.EmployeeIdAsync(),
                approvingUserIsAdmin : User.IsInRole(UserRoleName.Admin),
                employeeId: request.EmployeeId,
                weekId : request.WeekId,
                newApprovalState : request.NewApprovalState
            );
            var res = await approveTimeCommand.ApplyApproval(req);
            if(res.Successful)
            {
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Timesheet is {request.NewApprovalState}");
                return Ok();
            }
            else
            {
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{string.Join(",",res.Errors)}");
                return Unauthorized();
            }
            
        }
    }
}

