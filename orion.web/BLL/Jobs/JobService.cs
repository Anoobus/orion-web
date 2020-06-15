using Microsoft.EntityFrameworkCore;
using orion.web.Clients;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Jobs
{
    public interface IJobService
    {
        Task<JobDTO> GetForJobId(int jobId);
        Task<IEnumerable<JobDTO>> GetAsync(int employeeId);
        Task<IEnumerable<JobDTO>> GetAsync();
        JobDTO Post(JobDTO job);
        Task PutAsync(JobDTO jobDto);
        Task<IEnumerable<JobStatusDTO>> GetUsageStatusAsync();
    }
    public class JobService : IJobService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public JobService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<JobStatusDTO>> GetUsageStatusAsync()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.JobStatuses.Select(x => new JobStatusDTO()
                {
                    Id = x.JobStatusId,
                    Name = x.Name,
                    Enum = (Jobs.JobStatus)x.JobStatusId
                }).ToListAsync();
            }
        }

        public async Task<IEnumerable<JobDTO>> GetAsync(int employeeId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var thisEmpJobsGlob = await db.Employees.Include(x => x.EmployeeJobs).SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
                var thisEmpJobs = thisEmpJobsGlob?.EmployeeJobs?.Select(z => z.JobId)?.ToArray();
                var basePull = await db.Jobs.Include(x => x.Client)
                                    .Include(x => x.Site)
                                    .Include(x => x.JobStatus)
                                    .Where(x => thisEmpJobs.Contains(x.JobId))
                                    .Select(x => new { Job = x, x.Site, x.Client })
                                    .ToListAsync();
                var mapped = new List<JobDTO>();
                foreach(var item in basePull)
                {
                    mapped.Add(MapToDTO(item.Job));
                }
                return mapped.OrderBy(x => x.FullJobCodeWithName).ToList();
            }
        }

        public async Task<IEnumerable<JobDTO>> GetAsync()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.Jobs
                         .Include(x => x.Client)
                         .Include(x => x.Site)
                         .Include(x => x.JobStatus)
                         .Select(Job => MapToDTO(Job)).ToListAsync();
            }
        }

        public async Task<JobDTO> GetForJobId(int jobId)
        {
            using(var db = _contextFactory.CreateDb())
            {
                return (await db.Jobs
                         .Include(x => x.Client)
                         .Include(x => x.Site)
                         .Include(x => x.JobStatus)
                         .Include(x => x.ProjectManager)
                         .Where(x => x.JobId == jobId)
                         .Select(Job => MapToDTO(Job)).ToListAsync()).FirstOrDefault();
            }
        }

        private static JobDTO MapToDTO(Job Job)
        {
            return new JobDTO()
            {
                JobCode = Job.JobCode,
                JobId = Job.JobId,
                JobName = Job.JobName,
                Client = new ClientDTO()
                {
                    ClientCode = Job.Client.ClientCode,
                    ClientId = Job.Client.ClientId,
                    ClientName = Job.Client.ClientName
                },
                Site = new SiteDTO()
                {
                    SiteID = Job.Site.SiteID,
                    SiteName = Job.Site.SiteName
                },
                TargetHours = Job.TargetHours,
                JobStatusDTO = new JobStatusDTO()
                {
                    Enum = (Jobs.JobStatus)Job.JobStatus.JobStatusId,
                    Id = Job.JobStatus.JobStatusId,
                    Name = Job.JobStatus.Name
                },
                ProjectManager = new ProjectManagerDTO()
                {
                    EmployeeId = Job.EmployeeId,
                    EmployeeName = Job?.ProjectManager != null ? $"{Job.ProjectManager.Last}, {Job.ProjectManager.First}" : string.Empty
                }
            };
        }

        public JobDTO Post(JobDTO job)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var efJob = new Job()
                {
                    ClientId = job.Client.ClientId,
                    JobCode = job.JobCode,
                    JobName = job.JobName,
                    SiteId = job.Site.SiteID,
                    TargetHours = job.TargetHours,
                    JobStatusId = job.JobStatusDTO.Id,
                    EmployeeId = job.ProjectManager.EmployeeId
                };
                db.Jobs.Add(efJob);
                db.SaveChanges();
                job.JobId = efJob.JobId;
                return job;
            }
        }

        public async Task PutAsync(JobDTO job)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var efJob = db.Jobs.Single(x => x.JobId == job.JobId);
                efJob.ClientId = job.Client.ClientId;
                efJob.JobCode = job.JobCode;
                efJob.JobName = job.JobName;
                efJob.SiteId = job.Site.SiteID;
                efJob.TargetHours = job.TargetHours;
                efJob.JobStatusId = job.JobStatusDTO.Id;
                efJob.EmployeeId = job.ProjectManager.EmployeeId;
                if(job.JobStatusDTO.Id == (int)(Jobs.JobStatus.Archived))
                {
                    var toRemove = await db.EmployeeJobs.Where(x => x.JobId == job.JobId).ToArrayAsync();
                    db.EmployeeJobs.RemoveRange(toRemove);
                }
                db.SaveChanges();
            }
        }
    }
}
