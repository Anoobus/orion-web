using AutoMapper;
using Microsoft.EntityFrameworkCore;
using orion.web.BLL.Jobs;
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
    public interface IJobsRepository
    {
        Task<JobDTO> GetForJobId(int jobId);
        Task<IEnumerable<JobDTO>> GetAsync(int employeeId);
        Task<IEnumerable<JobDTO>> GetAsync();
        Task<JobDTO> Create(CreateJobDto job);
        Task<JobDTO> Update(UpdateJobDto jobDto);
        Task<IEnumerable<JobStatusDTO>> GetUsageStatusAsync();
    }
    public class JobsRepository : IJobsRepository, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public JobsRepository(IContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
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

        private JobDTO MapToDTO(Job Job)
        {
            return _mapper.Map<JobDTO>(Job);
            //return new JobDTO()
            //{
            //    JobCode = Job.JobCode,
            //    JobId = Job.JobId,
            //    JobName = Job.JobName,
            //    //Client = _mapper.Map<ClientDTO>(Job.Client),
            //    //Site = new SiteDTO()
            //    //{
            //    //    SiteID = Job.Site.SiteID,
            //    //    SiteName = Job.Site.SiteName
            //    //},
            //    TargetHours = Job.TargetHours,
            //    //JobStatusDTO = new JobStatusDTO()
            //    //{
            //    //    Enum = (Jobs.JobStatus)Job.JobStatus.JobStatusId,
            //    //    Id = Job.JobStatus.JobStatusId,
            //    //    Name = Job.JobStatus.Name
            //    //},
            //    //ProjectManager = new ProjectManagerDTO()
            //    //{
            //    //    EmployeeId = Job.EmployeeId,
            //    //    EmployeeName = Job?.ProjectManager != null ? $"{Job.ProjectManager.Last}, {Job.ProjectManager.First}" : string.Empty
            //    //}
            //};
        }

        public async Task<JobDTO> Create(CreateJobDto job)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var efJob = _mapper.Map<Job>(job);
                db.Jobs.Add(efJob);
                await db.SaveChangesAsync();

                return await GetForJobId(efJob.JobId);
            }
        }

        public async Task<JobDTO> Update(UpdateJobDto job)
        {
            using(var db = _contextFactory.CreateDb())
            {
                var efJob = db.Jobs.Single(x => x.JobId == job.JobId);
                efJob.ClientId = job.ClientId;
                efJob.JobCode = job.JobCode;
                efJob.JobName = job.JobName;
                efJob.SiteId = job.SiteId;
                efJob.TargetHours = job.TargetHours;
                efJob.JobStatusId = (int)job.JobStatusId;
                efJob.EmployeeId = job.ProjectManagerEmployeeId;
                if(job.JobStatusId == Jobs.JobStatus.Archived)
                {
                    var toRemove = await db.EmployeeJobs.Where(x => x.JobId == job.JobId).ToArrayAsync();
                    db.EmployeeJobs.RemoveRange(toRemove);
                }
                await db.SaveChangesAsync();
                return await GetForJobId(efJob.JobId);
            }
        }
    }
}
