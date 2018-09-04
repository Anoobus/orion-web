using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using orion.web.Clients;
using orion.web.DataAccess.EF;
using orion.web.JobsTasks;

namespace orion.web.Jobs
{
    public interface IJobService
    {
        IEnumerable<JobDTO> Get(string ForEmployeeName);
        IEnumerable<JobDTO> Get();
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

        public IEnumerable<JobDTO> Get(string ForEmployeeName)
        {
            var thisEmpJobs = db.Employees.Include(x => x.EmployeeJobs).SingleOrDefault(x => x.Name == ForEmployeeName)?.EmployeeJobs?.Select(z => z.JobId)?.ToArray();
            return db.Jobs.Where(x => thisEmpJobs.Contains(x.JobId))
                     .Include(x => x.Client)
                     .Include(x => x.Site)
                     .Include(x => x.TaskCategory)
                     ?.Select(job => MapToDTO(job))
                     ?.ToList() ?? new List<JobDTO>();

        }

        public IEnumerable<JobDTO> Get()
        {
            return db.Jobs
                         .Include(x => x.Client)
                         .Include(x => x.Site)
                         .Include(x => x.TaskCategory)
                         .Select(Job => MapToDTO(Job)).ToList();
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
                AllowedCategory = Enum.Parse<TaskCategoryId>(Job.TaskCategory.Name,true),
                 TargetHours = Job.TargetHours  
            };
        }

        public JobDTO Post(JobDTO job)
        {
            if (job.AllowedCategory == TaskCategoryId.unknown)
            {
                job.AllowedCategory = TaskCategoryId.normal;
            }
            var efJob = new Job()
            {
                ClientId = job.Client.ClientId,
                JobCode = job.JobCode,
                JobName = job.JobName,
                SiteId = job.Site.SiteID,
                TaskCategoryId = db.TaskCategories.Single(x => x.Name ==  job.AllowedCategory.ToString()).TaskCategoryId,
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
            efJob.TaskCategoryId = (int)job.AllowedCategory;
            efJob.TargetHours = job.TargetHours;
            db.SaveChanges();
        }
    }
}
