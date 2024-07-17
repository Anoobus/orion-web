using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Notifications;
using Orion.Web.TimeApproval;

namespace Orion.Web.TimeEntries
{
    [Authorize]
    public class TimeApprovalController : Controller
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IApproveTimeCommand approveTimeCommand;
        private readonly IWeekOfTimeEntriesQuery weekOfTimeEntriesQuery;
        private readonly ITimeApprovalListQuery timeApprovalListQuery;
        private readonly ISessionAdapter sessionAdapter;

        public TimeApprovalController(
            ITimeApprovalService timeApprovalService,
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

        public const string APPROVALROUTE = "APPROVAL_ROUTE";

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

        [HttpGet("[Controller]/Hide/{employeeId:int}/week/{weekId:int}")]
        public async Task<ActionResult> HideEntry([FromRoute] int employeeId, [FromRoute] int weekId)
        {
            await timeApprovalService.Hide(weekId, employeeId);
            var vm = await timeApprovalListQuery.GetApprovalListAsync();
            return View("List", vm);
        }

        [HttpPost]
        public async Task<ActionResult> Index(DateTime PeriodStartData, DateTime PeriodEndDate, ActiveSection activeSection)
        {
            var vm = await timeApprovalListQuery.GetApprovalListAsync(beginDate: PeriodStartData, endDate: PeriodEndDate, activeSection: activeSection);
            return View("List", vm);
        }

        [HttpPost]
        [Route("TimeApproval/{newApprovalState}", Name = APPROVALROUTE)]
        public async Task<ActionResult> ApplyApproval(TimeApprovalModel request)
        {
            var req = new TimeApprovalRequest(
                approvingUserId: await sessionAdapter.EmployeeIdAsync(),
                approvingUserIsAdmin: User.IsInRole(UserRoleName.Admin),
                employeeId: request.EmployeeId,
                weekId: request.WeekId,
                newApprovalState: request.NewApprovalState
            );
            var res = await approveTimeCommand.ApplyApproval(req);
            if (res.Successful)
            {
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Timesheet is {request.NewApprovalState}");
                return Ok();
            }
            else
            {
                NotificationsController.AddNotification(this.User.SafeUserName(), $"{string.Join(",", res.Errors)}");
                return Unauthorized();
            }
        }
    }
}
