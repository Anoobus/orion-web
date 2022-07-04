using System;
using System.Threading.Tasks;
using orion.web.Util.IoC;

namespace orion.web.BLL.TimeApproval
{
    
        public interface IEmailExclusionFilter
        {
            Task<bool> ShouldRecieveProjectManagerTimeSubmittedEmail(int employeeId);
            Task<bool> ShouldRecieveSubmittTimeReminder(int employeeId);     
        }
        public class EmailExclusionFilter: IEmailExclusionFilter, IAutoRegisterAsSingleton
        {          
            public const int CHRIS_EMPLOYEE_ID = 2;
            public Task<bool> ShouldRecieveProjectManagerTimeSubmittedEmail(int employeeId)
            {
                return Task.FromResult(employeeId == CHRIS_EMPLOYEE_ID);
            }

            public Task<bool> ShouldRecieveSubmittTimeReminder(int employeeId)
            {
                 return Task.FromResult(employeeId == CHRIS_EMPLOYEE_ID);
      
            }
    }
    
}

