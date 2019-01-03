using Microsoft.EntityFrameworkCore;
using orion.web.Clients;
using orion.web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Jobs
{
    public interface IJobService
    {
        Task<IEnumerable<JobDTO>> GetAsync(int employeeId);
        Task<IEnumerable<JobDTO>> GetAsync();
        JobDTO Post(JobDTO job);
        void Put(JobDTO jobDto);
    }
    public class JobService : IJobService
    {
        private readonly OrionDbContext db;

        public JobService(OrionDbContext orionDbContext)
        {
            this.db = orionDbContext;
        }

        public async Task<IEnumerable<JobDTO>> GetAsync(int employeeId)
        {
            var thisEmpJobsGlob = await db.Employees.Include(x => x.EmployeeJobs).SingleOrDefaultAsync(x => x.EmployeeId == employeeId);
            var thisEmpJobs = thisEmpJobsGlob?.EmployeeJobs?.Select(z => z.JobId)?.ToArray();
            return await db.Jobs.Where(x => thisEmpJobs.Contains(x.JobId))
                     .Include(x => x.Client)
                     .Include(x => x.Site)
                     ?.Select(job => MapToDTO(job))
                     ?.ToListAsync() ?? new List<JobDTO>();

        }

        public async Task<IEnumerable<JobDTO>> GetAsync()
        {
            return await db.Jobs
                         .Include(x => x.Client)
                         .Include(x => x.Site)
                         .Select(Job => MapToDTO(Job)).ToListAsync();
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
                TargetHours = Job.TargetHours
            };
        }

        public JobDTO Post(JobDTO job)
        {
            var efJob = new Job()
            {
                ClientId = job.Client.ClientId,
                JobCode = job.JobCode,
                JobName = job.JobName,
                SiteId = job.Site.SiteID,
                TargetHours = job.TargetHours
            };
            db.Jobs.Add(efJob);
            db.SaveChanges();
            job.JobId = efJob.JobId;
            return job;
        }

        public void Put(JobDTO job)
        {
            var efJob = db.Jobs.Single(x => x.JobId == job.JobId);
            efJob.ClientId = job.Client.ClientId;
            efJob.JobCode = job.JobCode;
            efJob.JobName = job.JobName;
            efJob.SiteId = job.Site.SiteID;
            efJob.TargetHours = job.TargetHours;
            db.SaveChanges();
        }
    }
}
