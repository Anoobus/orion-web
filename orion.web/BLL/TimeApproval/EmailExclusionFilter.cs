using System;
using System.Threading.Tasks;
using Orion.Web.Util.IoC;

namespace Orion.Web.BLL.TimeApproval
{
    public interface IEmailExclusionFilter
    {
        Task<bool> ShouldRecieveProjectManagerTimeSubmittedEmail(int employeeId);
        Task<bool> ShouldRecieveSubmittTimeReminder(int employeeId);
    }

    public class EmailExclusionFilter : IEmailExclusionFilter, IAutoRegisterAsSingleton
    {
        public const int CHRISEMPLOYEEID = 2;
        public Task<bool> ShouldRecieveProjectManagerTimeSubmittedEmail(int employeeId)
        {
            return Task.FromResult(employeeId == CHRISEMPLOYEEID);
        }

        public Task<bool> ShouldRecieveSubmittTimeReminder(int employeeId)
        {
            return Task.FromResult(employeeId == CHRISEMPLOYEEID);
        }
    }
}
