using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    [Authorize]
    [Route("orion-api/v1/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IInMemRepo<Employee> _repo;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {

            _repo = new InMemRepo<Employee>(() => ((Employee z) => z.Id, employeeRepository.GetAllEmployees().GetAwaiter().GetResult().Select(x => new Employee()
            {
                 Email = x.UserName,
                  FirstName = x.First,
                   LastName = x.Last,
                Id = x.UserName.ToGuid(),
            }).ToArray()));
        }

        [HttpGet()]
        public async Task<ActionResult<Page<Employee>>> Get([FromQuery] int? limit,
        [FromQuery] int? offset)
        {
            var matches = _repo.Search(x => true).ToArray();
            return Ok(new Page<Employee>()
            {
                Data = matches,
                Meta = new Meta()
                {
                    Limit = int.MaxValue,
                    Offset = 0,
                    Total = matches.Length
                }
            });
        }
    }
}
