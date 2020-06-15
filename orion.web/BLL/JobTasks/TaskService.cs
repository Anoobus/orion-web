using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess.EF;
using orion.web.JobTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.JobsTasks
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDTO>> GetTasks();
        Task<IEnumerable<UsageStatusDTO>> GetUsageStatusAsync();
        Task<IEnumerable<CategoryDTO>> GetTaskCategoriesAsync();
        Task Upsert(TaskDTO task);
    }

    public class TaskService : ITaskService
    {
        private readonly OrionDbContext db;

        public TaskService(OrionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CategoryDTO>> GetTaskCategoriesAsync()
        {
            return await db.TaskCategories.Select(x => new CategoryDTO()
            {
                Id = x.TaskCategoryId,
                Name = x.Name,
                Enum = (JobTasks.TaskCategory)x.TaskCategoryId
            }).ToListAsync();
        }


        public async Task<IEnumerable<UsageStatusDTO>> GetUsageStatusAsync()
        {
            return await db.UsageStatuses.Select(x => new UsageStatusDTO()
            {
                Id = x.UsageStatusId,
                Name = x.Name,
                Enum = (JobTasks.UsageStatus)x.UsageStatusId
            }).ToListAsync();
        }

        public async Task<IEnumerable<TaskDTO>> GetTasks()
        {
            return (await db.JobTasks
                .Include(x => x.TaskCategory)
                .Include(x => x.UsageStatus)
                .Select(x => new TaskDTO()
                {
                    Description = x.Description,
                    Name = x.Name,
                    Category = new CategoryDTO()
                    {
                        Enum = (JobTasks.TaskCategory)x.TaskCategory.TaskCategoryId,
                        Id = x.TaskCategory.TaskCategoryId,
                        Name = x.TaskCategory.Name
                    },
                    LegacyCode = x.LegacyCode,
                    UsageStatus = new UsageStatusDTO()
                    {
                        Enum = (JobTasks.UsageStatus)x.UsageStatusId,
                        Id = x.UsageStatus.UsageStatusId,
                        Name = x.UsageStatus.Name
                    },
                    TaskId = x.JobTaskId,
                }).ToListAsync())
                .OrderBy(x => x.Category.Name)
                .ThenBy(x => x.LegacyCode)
                .ThenBy(x => x.Name);
        }

        public async Task Upsert(TaskDTO task)
        {
            var match = await db.JobTasks.SingleOrDefaultAsync(x => x.JobTaskId == task.TaskId);
            if(match == null)
            {
                match = new JobTask();
                db.JobTasks.Add(match);
                if(await db.JobTasks.AnyAsync(x => x.LegacyCode == task.LegacyCode))
                {
                    throw new ArgumentException($"Supplied code {task.LegacyCode} is already in use");
                }
            }
            else
            {
                if(await db.JobTasks.AnyAsync(x => x.LegacyCode == match.LegacyCode && x.JobTaskId != task.TaskId))
                {
                    throw new ArgumentException($"Supplied code {match.LegacyCode} is already in use");
                }
            }
            

            match.Description = task.Description;
            match.LegacyCode = task.LegacyCode;
            match.Name = task.Name;
            match.TaskCategoryId = task.Category.Id;
            match.UsageStatusId = task.UsageStatus.Id;
            db.SaveChanges();
        }

    }
}
