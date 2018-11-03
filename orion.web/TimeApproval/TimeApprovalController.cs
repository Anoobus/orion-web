using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeApproval;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{

    [Authorize]
    public class TimeApprovalController : Controller
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly IApproveTimeCommand approveTimeCommand;

        public TimeApprovalController(ITimeApprovalService timeApprovalService, IApproveTimeCommand approveTimeCommand)
        {
            this.timeApprovalService = timeApprovalService;
            this.approveTimeCommand = approveTimeCommand;
        }

        public const string APPROVAL_ROUTE = "APPROVAL_ROUTE";

        public class TimeApprovalRequest
        {
            public int Year { get; set; }
            public int WeekId { get; set; }
            public string AccountName { get; set; }
            public TimeApprovalStatus NewApprovalState { get; set; }
        }

        //public ActionResult Index()
        //{
        //    //return View("WeekList", weekIdentifierListQuery.GetWeeks(5));
        //}

        [HttpPost]
        [Route("TimeApproval/{newApprovalState}", Name = APPROVAL_ROUTE)]
        public async Task<ActionResult> ApplyApproval(TimeApprovalRequest request)
        {
            var res = await approveTimeCommand.ApplyApproval(User.IsInRole(UserRoleName.Admin), User.Identity.Name, request);
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

