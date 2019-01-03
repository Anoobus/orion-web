using Microsoft.AspNetCore.Http;
using orion.web.Employees;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface ISessionAdapter : IRegisterByConvention
    {
        Task<int> EmployeeIdAsync();
    }

    public class SessionAdapter : ISessionAdapter
    {
        private readonly IEmployeeService employeeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Task<string>>> InProcessSessionStore = new ConcurrentDictionary<string, ConcurrentDictionary<string, Task<string>>>(); 
        public SessionAdapter(IEmployeeService employeeService, IHttpContextAccessor httpContextAccessor)
        {
            this.employeeService = employeeService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> EmployeeIdAsync()
        {
            var dict = InProcessSessionStore.GetOrAdd(httpContextAccessor.HttpContext.User.Identity.Name, new ConcurrentDictionary<string, Task<string>>());
            var employeeId = await dict.GetOrAdd("USER_ID", async (key) =>
            {
                var employeeInfo = await employeeService.GetSingleEmployeeAsync(httpContextAccessor.HttpContext.User.Identity.Name);
                return employeeInfo.EmployeeId.ToString();
            });
            return int.Parse(employeeId);
        }

    }
}
