using orion.web.Common;
using orion.web.TimeEntries;
using System;
using System.Linq;
using System.Threading.Tasks;
using static orion.web.TimeEntries.TimeApprovalController;

namespace orion.web.TimeApproval
{
    public interface IApproveTimeCommand : IRegisterByConvention
    {
        Task<CommandResult> ApplyApproval(bool approverIsAdmin, string approvingEmployeeName, TimeApprovalRequest request);
    }

    public class ApproveTimeCommand : IApproveTimeCommand
    {
        private readonly ITimeApprovalService timeApprovalService;
        private readonly ITimeService timeService;

        public ApproveTimeCommand(ITimeApprovalService timeApprovalService, ITimeService timeService)
        {
            this.timeApprovalService = timeApprovalService;
            this.timeService = timeService;
        }

        public async Task<CommandResult> ApplyApproval(bool approverIsAdmin, string approvingEmployeeName, TimeApprovalRequest request)
        {
            var current = await timeApprovalService.Get(request.Year, request.WeekId, request.AccountName);
            var isValidSubmit = request.NewApprovalState == TimeEntries.TimeApprovalStatus.Submitted &&
                !(current.TimeApprovalStatus == TimeApprovalStatus.Submitted || current.TimeApprovalStatus == TimeApprovalStatus.Approved);
            var isValidReject = request.NewApprovalState == TimeEntries.TimeApprovalStatus.Rejected
                && (current.TimeApprovalStatus == TimeApprovalStatus.Submitted || current.TimeApprovalStatus == TimeApprovalStatus.Approved)
                && approverIsAdmin;
            var isValidApprove = request.NewApprovalState == TimeApprovalStatus.Approved
                && approverIsAdmin;

            var sendNotification = false;
            if(isValidSubmit)
            {
                var time = await timeService.GetAsync(request.Year, request.WeekId, request.AccountName);
                var totalOt = time.Sum(x => x.OvertimeHours);
                var totalReg = time.Sum(x => x.Hours);
                current.TotalOverTimeHours = totalOt;
                current.TotalRegularHours = totalReg;
                
                if(totalOt > 0)
                {
                    current.TimeApprovalStatus = TimeApprovalStatus.Submitted;
                    sendNotification = true;
                }
                else
                {
                    current.TimeApprovalStatus = TimeApprovalStatus.Approved;
                }
            }
            if(isValidReject || isValidApprove)
            {
                current.TimeApprovalStatus = request.NewApprovalState;
            }
            if(isValidApprove)
            {
                current.ApproverName = approvingEmployeeName;
                current.ApprovalDate = DateTime.Now;
            }
            if(sendNotification)
            {
                //SmtpProxy.Test();
            }
            await timeApprovalService.Save(current);
            return new CommandResult(true);
         
        }
    }
}
