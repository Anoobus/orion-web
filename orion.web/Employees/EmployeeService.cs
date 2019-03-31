using Microsoft.EntityFrameworkCore;
using orion.web.Common;
using orion.web.DataAccess.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Employees
{

    public interface IEmployeeService : IRegisterByConvention
    {
        Task<IEnumerable<string>> GetAllRoles();
        Task<EmployeeDTO> GetSingleEmployeeAsync(string employeeName);
        Task<EmployeeDTO> GetSingleEmployeeAsync(int employeeId);
        Task<IEnumerable<EmployeeDTO>> GetAllEmployees();
        void Save(EmployeeDTO employee);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly OrionDbContext db;

        public EmployeeService(OrionDbContext db)
        {
            this.db = db;
        }
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployees()
        {
            return await db.Employees
                     .Include(x => x.EmployeeJobs)
                     .Include(x => x.UserRole)
                     .Select(x => MapToDTO(x)).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            return await db.UserRoles.Select(x => x.Name).ToListAsync();
        }

        public async Task<EmployeeDTO> GetSingleEmployeeAsync(string employeeName)
        {
            return MapToDTO(await db.Employees
                          .Include(x => x.EmployeeJobs)
                          .Include(x => x.UserRole)
                          .FirstOrDefaultAsync(x => x.UserName == employeeName));
        }

        public async Task<EmployeeDTO> GetSingleEmployeeAsync(int employeeId)
        {

            return MapToDTO(await db.Employees
                          .Include(x => x.EmployeeJobs)
                          .Include(x => x.UserRole)
                          .FirstOrDefaultAsync(x => x.EmployeeId == employeeId));
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

        public void Save(EmployeeDTO employee)
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

            db.SaveChanges();
        }

    }
}
