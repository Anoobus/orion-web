using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Orion.Web.Common;
using Orion.Web.Employees;

namespace Orion.Web.BLL.Authorization
{
    public static class UserRolePermissions
    {
        public static bool CanManageTimeEntryApprovals(this ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole(UserRoleName.Admin);
        }

        public static bool CanManageExpenses(this ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole(UserRoleName.Admin);
        }

        public static bool CanManageAllEmployeesTime(this ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole(UserRoleName.Admin);
        }

        public static bool CanManageSystemConfiguration(this ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole(UserRoleName.Admin);
        }

        public static bool CanViewOtherUsersTime(this ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole(UserRoleName.Admin)
                   || currentUser.IsInRole(UserRoleName.Supervisor);
        }
    }

    public interface IDirectReportsRepository
    {
        Task<int[]> GetDirectReports(int currentUserId);
    }
}
