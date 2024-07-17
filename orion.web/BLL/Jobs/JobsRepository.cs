using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orion.Web.BLL.Jobs;
using Orion.Web.Clients;
using Orion.Web.DataAccess;
using Orion.Web.DataAccess.EF;
using Orion.Web.Util.IoC;

namespace Orion.Web.Jobs
{
    public interface IJobsRepository
    {
        Task<JobDto> GetForJobId(int jobId);
        Task<IEnumerable<CoreJobDto>> GetAsync(int employeeId);
        Task<IEnumerable<CoreJobDto>> GetAsync();
        Task<CoreJobDto> Create(CreateJobDto job);
        Task<CoreJobDto> Update(UpdateJobDto jobDto);
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
            using (var db = _contextFactory.CreateDb())
            {
                return await db.JobStatuses.Select(x => new JobStatusDTO()
                {
                    Id = x.JobStatusId,
                    Name = x.Name,
                    Enum = (Jobs.JobStatus)x.JobStatusId
                }).ToListAsync();
            }
        }

        public async Task<IEnumerable<CoreJobDto>> GetAsync(int employeeId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var thisEmpJobsGlob = await db.Employees.Include(x => x.EmployeeJobs).SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
                var thisEmpJobs = thisEmpJobsGlob?.EmployeeJobs?.Select(z => z.JobId)?.ToArray();
                var basePull = await db.Jobs.Include(x => x.Client)
                                    .Include(x => x.Site)
                                    .Include(x => x.JobStatus)
                                    .Where(x => thisEmpJobs.Contains(x.JobId))
                                    .Select(x => new { Job = x, x.Site, x.Client })
                                    .ToListAsync();
                var mapped = new List<CoreJobDto>();
                foreach (var item in basePull)
                {
                    mapped.Add(_mapper.Map<CoreJobDto>(item.Job));
                }

                return mapped.OrderBy(x => x.FullJobCodeWithName).ToList();
            }
        }

        public async Task<IEnumerable<CoreJobDto>> GetAsync()
        {
            using (var db = _contextFactory.CreateDb())
            {
                return (await db.Jobs.ToListAsync())
                                     .Select(x => _mapper.Map<CoreJobDto>(x))
                                     .ToList();
            }
        }

        public async Task<JobDto> GetForJobId(int jobId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return (await db.Jobs
                         .Include(x => x.Client)
                         .Include(x => x.Site)
                         .Include(x => x.JobStatus)
                         .Include(x => x.ProjectManager)
                         .Where(x => x.JobId == jobId)
                         .Select(Job => MapToDTO(Job, _mapper)).ToListAsync()).FirstOrDefault();
            }
        }

        private static JobDto MapToDTO(Job Job, IMapper mapper)
        {
            return new JobDto()
            {
                CoreInfo = mapper.Map<CoreJobDto>(Job),
                Client = mapper.Map<ClientDTO>(Job.Client),
                Site = mapper.Map<SiteDTO>(Job.Site)
            };

            // TargetHours = Job.TargetHours,
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
            // };
        }

        public async Task<CoreJobDto> Create(CreateJobDto job)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var efJob = _mapper.Map<Job>(job);
                db.Jobs.Add(efJob);
                await db.SaveChangesAsync();

                return (await GetForJobId(efJob.JobId)).CoreInfo;
            }
        }

        public async Task<CoreJobDto> Update(UpdateJobDto job)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var efJob = db.Jobs.Single(x => x.JobId == job.JobId);
                efJob.ClientId = job.ClientId;
                efJob.JobCode = job.JobCode;
                efJob.JobName = job.JobName;
                efJob.SiteId = job.SiteId;
                efJob.TargetHours = job.TargetHours;
                efJob.JobStatusId = (int)job.JobStatusId;
                efJob.EmployeeId = job.ProjectManagerEmployeeId;
                if (job.JobStatusId == Jobs.JobStatus.Archived)
                {
                    var toRemove = await db.EmployeeJobs.Where(x => x.JobId == job.JobId).ToArrayAsync();
                    db.EmployeeJobs.RemoveRange(toRemove);
                }

                await db.SaveChangesAsync();
                return (await GetForJobId(efJob.JobId)).CoreInfo;
            }
        }
    }
}
