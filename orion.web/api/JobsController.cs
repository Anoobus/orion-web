using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.api.expenditures;
using orion.web.api.expenditures.Models;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.api
{
    public class JobOneTimeSet : EditableJob
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
    }


    public class EditableJob
    {
        public string JobCode { get; set; }
        public string Name { get; set; }
        public Guid SiteId { get; set; }
        public Guid ClientId { get; set; }
    }

    public class Job : JobOneTimeSet
    {

    }

    [Authorize]
    [Route("orion-api/v1/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IInMemRepo<Job> _repo;
        public JobsController( IMapper mapper, IJobsRepository jobRepo)
        {
            _repo = new InMemRepo<Job>(() => ((Job z) => z.Id, jobRepo.GetAsync()
                                                                      .GetAwaiter()
                                                                      .GetResult()
                                                                      .Select(x => new Job()
            {
                Name = x.JobName,
                JobCode = x.JobCode,
                Id = x.FullJobCodeWithName.ToGuid(),
                ClientId = x.ClientId.ToString().ToGuid(),
                SiteId = x.SiteId.ToString().ToGuid(),
                LastModified = DateTimeOffset.Now,
            }).ToArray()));
        }

        [HttpGet()]
        public async Task<ActionResult<Page<Job>>> Get([FromQuery] int? limit,
            [FromQuery] int? offset)
        {
            var matches = _repo.Search(x => true).Skip(offset ?? 0).Take(limit ?? 25).ToArray();
            return Ok(new Page<Job>()
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


        [HttpPost()]
        public async Task<ActionResult<MiscExpenditure>> CreateExpenditure([FromBody] EditableJob coolz
           )
        {
            return Ok(_repo.AddOrUpdate(new Job()
            {
                Id = Guid.NewGuid(),
                LastModified = DateTimeOffset.Now,
                ClientId = coolz.ClientId,
                JobCode = coolz.JobCode,
                Name = coolz.Name,
                SiteId = coolz.SiteId
            }, x => x.Id));
        }


        [HttpPut("{job-id:Guid}")]
        public async Task<StatusCodeResult> UpdateExpenditure([FromBody] EditableJob coolz,
           [FromRoute(Name = "job-id")] Guid jobId)
        {
            var match = _repo.FindById(jobId);
            _repo.AddOrUpdate(new Job()
            {
                Id = match.Id,
                LastModified = DateTimeOffset.Now,
                ClientId = coolz.ClientId,
                JobCode = coolz.JobCode,
                Name = coolz.Name,
                SiteId = coolz.SiteId
            }, x => x.Id);
            return Ok();
        }
    }
}
