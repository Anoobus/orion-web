using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess.EF;

namespace orion.web.Employees
{

    public interface IEmployeeService
    {
        Task<IEnumerable<string>> GetAllRoles();
        EmployeeDTO GetSingleEmployee(string employeeName);
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
                     .Select(x => new EmployeeDTO()
                     {
                         EmployeeId = x.EmployeeId,
                         Name = x.Name,
                         AssignJobs = x.EmployeeJobs.Select(z => z.JobId).ToList(),
                          Role = x.UserRole.Name
                     }).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            return await db.UserRoles.Select(x => x.Name).ToListAsync();
        }

        public EmployeeDTO GetSingleEmployee(string employeeName)
        {

            return db.Employees
                          .Include(x => x.EmployeeJobs)
                          .Include(x => x.UserRole)
                          .Select(x => new EmployeeDTO()
                          {
                              EmployeeId = x.EmployeeId,
                              AssignJobs = x.EmployeeJobs.Select(z => z.JobId).ToList(),
                              Name = x.Name,
                               Role = x.UserRole.Name
                          })
                          .FirstOrDefault(x => x.Name == employeeName);


        }

        public void Save(EmployeeDTO employee)
        {
            var match = db.Employees
                          .Include(x => x.EmployeeJobs)
                          .FirstOrDefault(x => x.Name == employee.Name);

            if(match == null)
            {
                match = new Employee()
                {
                    Name = employee.Name,
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
            db.SaveChanges();
        }

    }
}
