﻿using orion.web.Common;
using orion.web.Employees;
using orion.web.TimeEntries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeApproval
{
    public class TimeApprovalRequest
    {
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

        public ApproveTimeCommand(ITimeApprovalService timeApprovalService, ITimeService timeService, IEmployeeService employeeService, ISmtpProxy smtpProxy)
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
            this.employeeService = employeeService;
            this.smtpProxy = smtpProxy;
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

            var sendNotification = false;
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
                    sendNotification = true;
                }
                else
                {
                    current.TimeApprovalStatus = TimeApprovalStatus.Approved;
                }
            }
            if (isValidReject || isValidApprove)
            {
                current.TimeApprovalStatus = request.NewApprovalState;
                sendNotification = true;
            }
            if (isValidApprove)
            {
                var approver = await employeeService.GetSingleEmployeeAsync(request.ApprovingUserId);
                current.ApproverName = approver.UserName;
                current.ApprovalDate = DateTime.Now;
            }
            if (sendNotification)
            {
                var emp = await employeeService.GetSingleEmployeeAsync(request.EmployeeId);
                var recipient = emp.UserName;
                smtpProxy.SendMail("tim.thelen@gmail.com","THIS IS A TEST OF THE EMAIL SYSTEM", "THIS IS A SUBJECT FOR THE THING");
            }
            await timeApprovalService.Save(current);
            return new CommandResult(true);

        }
    }
}
