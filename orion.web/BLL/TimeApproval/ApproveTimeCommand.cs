using Microsoft.Extensions.Configuration;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Jobs;
using orion.web.TimeEntries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeApproval
{
    public class TimeApprovalRequest
    {
        public TimeApprovalRequest(int approvingUserId, 
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
    public interface IApproveTimeCommand : IRegisterByConvention
    {
        Task<CommandResult> ApplyApproval(TimeApprovalRequest request);
    }

    public class ApproveTimeCommand : IApproveTimeCommand
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly ITimeService timeService;
        private readonly IEmployeeService employeeService;
        private readonly ISmtpProxy smtpProxy;
        private readonly IConfiguration configuration;
        private readonly IJobService jobService;

        public ApproveTimeCommand(ITimeApprovalService timeApprovalService, ITimeService timeService, IEmployeeService employeeService, ISmtpProxy smtpProxy, IConfiguration configuration, IJobService jobService)
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
            this.employeeService = employeeService;
            this.smtpProxy = smtpProxy;
            this.configuration = configuration;
            this.jobService = jobService;
        }

        public async Task<CommandResult> ApplyApproval(TimeApprovalRequest request)
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
                var time = await timeService.GetAsync(request.WeekId, request.EmployeeId);
                var totalOt = time.Sum(x => x.OvertimeHours);
                var totalReg = time.Sum(x => x.Hours);
                current.TotalOverTimeHours = totalOt;
                current.TotalRegularHours = totalReg;

                if (totalOt > 0 || totalReg > 40)
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
                var approver = await employeeService.GetSingleEmployeeAsync(request.ApprovingUserId);
                current.ApproverName = approver.UserName;
                current.ApprovalDate = DateTime.Now;
            }
           
            await timeApprovalService.Save(current);

            var baseUrl = configuration.GetValue<string>("BaseHostAddress");
            if(isValidSubmit && current.TimeApprovalStatus == TimeApprovalStatus.Submitted)
            { 
                var time = await timeService.GetAsync(request.WeekId, request.EmployeeId);
                var JobsThatCauseApprovalRequired = time.Where(x => x.OvertimeHours > 0).GroupBy(x => x.JobId);
                var jobDetails = await Task.WhenAll(JobsThatCauseApprovalRequired.Select(async x => await jobService.GetForJobId(x.Key)));
                int[] projectManagersToNotifiy = jobDetails.Select(x => x.ProjectManager.EmployeeId).ToArray();
                var week = WeekDTO.CreateForWeekId(request.WeekId);
                foreach(var pm in projectManagersToNotifiy)
                {
                    var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                    var approver = await employeeService.GetSingleEmployeeAsync(pm);
                    var recipient = approver.UserName;

                    smtpProxy.SendMail(recipient, $"{emp.Last}, {emp.First} has submitted their timesheet for approval for week {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()}." +
                        Environment.NewLine + $"Timesheet can be located here: {baseUrl}/Edit/Employee/{emp.EmployeeId}/Week/{week.WeekId.Value}", $"Timesheet submitted for approval ({emp.UserName})");
                }
            }

            if(isValidReject)
            {
                var week = WeekDTO.CreateForWeekId(request.WeekId);
                var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                var approver = await employeeService.GetSingleEmployeeAsync(request.ApprovingUserId);
                var recipient = emp.UserName;
                smtpProxy.SendMail(recipient, $"Your time sheet for {week.WeekStart.ToShortDateString()}-{week.WeekEnd.ToShortDateString()} has been {request.NewApprovalState} by {approver.First}." +
                    Environment.NewLine + $"Timesheet can be located here: {baseUrl}/Edit/Employee/{emp.EmployeeId}/Week/{week.WeekId.Value}", $"Timesheet has been {request.NewApprovalState}");
            }
            return new CommandResult(true);

        }
    }
}
