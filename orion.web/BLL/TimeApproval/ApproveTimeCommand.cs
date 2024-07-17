using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Orion.Web.BLL.TimeApproval;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Jobs;
using Orion.Web.TimeEntries;
using Orion.Web.Util.IoC;

namespace Orion.Web.TimeApproval
{
    public class TimeApprovalRequest
    {
        public TimeApprovalRequest(
            int approvingUserId,
            bool approvingUserIsAdmin,
            int weekId,
            int employeeId,
            TimeApprovalStatus newApprovalState)
        {
            ApprovingUserId = approvingUserId;
            ApprovingUserIsAdmin = approvingUserIsAdmin;
            WeekId = weekId;
            EmployeeId = employeeId;
            NewApprovalState = newApprovalState;
        }

        public int ApprovingUserId { get; set; }
        public bool ApprovingUserIsAdmin { get; set; }
        public int WeekId { get; set; }
        public int EmployeeId { get; set; }
        public TimeApprovalStatus NewApprovalState { get; set; }
    }

    public interface IApproveTimeCommand
    {
        Task<Result> ApplyApproval(TimeApprovalRequest request);
    }

    public class ApproveTimeCommand : IApproveTimeCommand, IAutoRegisterAsSingleton
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly ITimeService timeService;
        private readonly IEmployeeRepository employeeService;
        private readonly ISmtpProxy smtpProxy;
        private readonly IConfiguration configuration;
        private readonly IJobsRepository jobService;
        private readonly IEmailExclusionFilter emailExclusionFilter;

        public ApproveTimeCommand(
            ITimeApprovalService timeApprovalService,
            ITimeService timeService,
            IEmployeeRepository employeeService,
            ISmtpProxy smtpProxy,
            IConfiguration configuration,
            IJobsRepository jobService,
            IEmailExclusionFilter emailExclusionFilter)
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
            this.employeeService = employeeService;
            this.smtpProxy = smtpProxy;
            this.configuration = configuration;
            this.jobService = jobService;
            this.emailExclusionFilter = emailExclusionFilter;
        }

        private async Task UpdateCurrentStateForSubmit(TimeApprovalRequest request, TimeApprovalDTO current)
        {
            var time = await timeService.GetAsync(request.WeekId, request.EmployeeId);
            var totalOt = time.Sum(x => x.OvertimeHours);
            var totalReg = time.Sum(x => x.Hours);
            current.TotalOverTimeHours = totalOt;
            current.TotalRegularHours = totalReg;
        }

        public async Task<Result> ApplyApproval(TimeApprovalRequest request)
        {
            var current = await timeApprovalService.GetAsync(request.WeekId, request.EmployeeId);
            var isValidSubmit = request.NewApprovalState == TimeEntries.TimeApprovalStatus.Submitted &&
                !(current.TimeApprovalStatus == TimeApprovalStatus.Submitted || current.TimeApprovalStatus == TimeApprovalStatus.Approved);
            var isValidReject = request.NewApprovalState == TimeEntries.TimeApprovalStatus.Rejected
                && (current.TimeApprovalStatus == TimeApprovalStatus.Submitted || current.TimeApprovalStatus == TimeApprovalStatus.Approved)
                && request.ApprovingUserIsAdmin;
            var isValidApprove = request.NewApprovalState == TimeApprovalStatus.Approved
                && request.ApprovingUserIsAdmin;

            if (isValidSubmit)
            {
                await UpdateCurrentStateForSubmit(request, current);
                if (current.TotalOverTimeHours > 0 || current.TotalRegularHours > 40)
                {
                    current.TimeApprovalStatus = TimeApprovalStatus.Submitted;
                }
                else
                {
                    current.TimeApprovalStatus = TimeApprovalStatus.Approved;
                }
            }

            if (isValidReject || isValidApprove)
            {
                current.TimeApprovalStatus = request.NewApprovalState;
            }

            if (isValidApprove)
            {
                if (!current.SubmittedDate.HasValue)
                {
                    await UpdateCurrentStateForSubmit(request, current);
                    current.SubmittedDate = DateTime.Now;
                }

                var approvedBy = request.ApprovingUserId == request.EmployeeId ? "[AUTO APPROVED BY SYSTEM]" : "approved by manager";
                var approver = await employeeService.GetSingleEmployeeAsync(request.ApprovingUserId);
                current.ApproverName = approver.UserName;
                current.ApprovalDate = DateTime.Now;
                var week = WeekDTO.CreateForWeekId(request.WeekId);
                var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                string finalEmailText = await CreateEmailBody(
                    week,
                    emp,
                    greetingName: emp.First,
                    action: $"Time sheet approved (for the week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()})",
                    actionBy: approvedBy,
                    followup: "No further action is required.");
                var recipient = emp.UserName;
                smtpProxy.SendMail(recipient, finalEmailText, $"Time approved for week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()}");
            }

            await timeApprovalService.Save(current);

            if (isValidSubmit && current.TimeApprovalStatus == TimeApprovalStatus.Submitted)
            {
                var time = await timeService.GetAsync(request.WeekId, request.EmployeeId);
                var JobsThatCauseApprovalRequired = time.Where(x => x.OvertimeHours > 0).GroupBy(x => x.JobId);
                var jobDetails = await Task.WhenAll(JobsThatCauseApprovalRequired.Select(async x => (await jobService.GetForJobId(x.Key)).CoreInfo));
                int[] projectManagersToNotifiy = jobDetails.Select(x => x.ProjectManagerEmployeeId).ToArray();
                var week = WeekDTO.CreateForWeekId(request.WeekId);
                foreach (var pm in projectManagersToNotifiy)
                {
                    var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                    if (!await emailExclusionFilter.ShouldRecieveProjectManagerTimeSubmittedEmail(emp.EmployeeId))
                    {
                        var approver = await employeeService.GetSingleEmployeeAsync(pm);
                        var recipient = approver.UserName;

                        string finalEmailText = await CreateEmailBody(
                            week,
                            emp,
                            greetingName: approver.First,
                            action: $"submitted for approval (for the week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()})",
                            actionBy: $"{emp.First} {emp.Last}",
                            followup: $"You will need to review the timesheet as you are marked as a project manager a job in this week.");

                        smtpProxy.SendMail(recipient, finalEmailText, $"Time submitted for week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()}");
                    }
                }
            }

            if (isValidReject)
            {
                var week = WeekDTO.CreateForWeekId(request.WeekId);
                var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                var approver = await employeeService.GetSingleEmployeeAsync(request.ApprovingUserId);
                var recipient = emp.UserName;

                string finalEmailText = await CreateEmailBody(
                    week,
                    emp,
                    greetingName: emp.First,
                    action: $"{request.NewApprovalState}",
                    actionBy: $"{approver.First} {approver.Last}",
                    followup: $"Please review and resubmit for the week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()}.");

                smtpProxy.SendMail(recipient, finalEmailText, $"Time {request.NewApprovalState} for week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()}");
            }

            return new Result(true);
        }

        private async Task<string> CreateEmailBody(WeekDTO week, EmployeeDTO emp, string greetingName, string action, string actionBy, string followup)
        {
            var baseUrl = configuration.GetValue<string>("BaseHostAddress");
            var template = await File.ReadAllTextAsync(Path.Combine("wwwroot", "email-template.html"));
            var finalEmailText = Regex.Replace(template, "(\\{\\{name}})", greetingName);
            finalEmailText = Regex.Replace(finalEmailText, "(\\{\\{action}})", action);
            finalEmailText = Regex.Replace(finalEmailText, "(\\{\\{action-by}})", actionBy);
            finalEmailText = Regex.Replace(finalEmailText, "(\\{\\{followup}})", followup);
            finalEmailText = Regex.Replace(finalEmailText, "(\\{\\{link}})", $"{baseUrl}/Edit/Employee/{emp.EmployeeId}/Week/{week.WeekId.Value}");
            return finalEmailText;
        }
    }
}
