using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{

    public interface IEmployeeRepository
    {
        Task<IEnumerable<string>> GetAllRoles();
        Task<EmployeeDTO> GetSingleEmployeeAsync(string employeeName);
        Task<EmployeeDTO> GetSingleEmployeeAsync(int employeeId);
        Task<IEnumerable<EmployeeDTO>> GetAllEmployees();
        void Save(EmployeeDTO employee, string updatedEmail = null);
    }
    public class EmployeeRepository : IEmployeeRepository, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public EmployeeRepository(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployees()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.Employees
                      .Include(x => x.EmployeeJobs)
                      .Include(x => x.UserRole)
                      .Select(x => MapToDTO(x)).ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.UserRoles.Select(x => x.Name).ToListAsync();
            }
        }

        public async Task<EmployeeDTO> GetSingleEmployeeAsync(string employeeName)
        {
            using(var db = _contextFactory.CreateDb())
            {
                return MapToDTO(await db.Employees
                          .Include(x => x.EmployeeJobs)
                          .Include(x => x.UserRole)
                          .FirstOrDefaultAsync(x => x.UserName == employeeName));
            }
        }

        public async Task<EmployeeDTO> GetSingleEmployeeAsync(int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                return MapToDTO(await db.Employees
                          .Include(x => x.EmployeeJobs)
                          .Include(x => x.UserRole)
                          .FirstOrDefaultAsync(x => x.EmployeeId == employeeId));
            }
        }

        private static EmployeeDTO MapToDTO(Employee x)
        {
            if(x == null)
            {
                return null;
            }
            return new EmployeeDTO()
            {
                EmployeeId = x.EmployeeId,
                AssignJobs = x.EmployeeJobs.Select(z => z.JobId).ToList(),
                UserName = x.UserName,
                First = x.First,
                IsExempt = x.IsExempt,
                Last = x.Last,
                Role = x.UserRole.Name
            };
        }

        public void Save(EmployeeDTO employee, string updatedEmail = null)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var match = db.Employees
                          .Include(x => x.EmployeeJobs)
                          .FirstOrDefault(x => x.UserName == employee.UserName);

                if(match == null)
                {
                    match = new Employee()
                    {
                        UserName = employee.UserName,
                        EmployeeId = employee.EmployeeId
                    };
                    db.Employees.Add(match);
                }
                match.EmployeeJobs.Clear();
                foreach(var jobId in employee.AssignJobs)
                {
                    match.EmployeeJobs.Add(new EmployeeJob()
                    {
                        JobId = jobId,
                    });
                }

                match.UserRoleId = db.UserRoles.Single(x => x.Name == employee.Role).UserRoleId;
                match.First = employee.First;
                match.Last = employee.Last;
                match.IsExempt = employee.IsExempt;
                if(!string.IsNullOrWhiteSpace(updatedEmail))
                    match.UserName = updatedEmail;

                db.SaveChanges();
            }
        }
    }
}
