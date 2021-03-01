using Microsoft.EntityFrameworkCore;
using orion.web.BLL.JobTasks;
using orion.web.DataAccess;
using orion.web.DataAccess.EF;
using orion.web.JobTasks;
using orion.web.Util.IoC;
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

    public class TaskService : ITaskService, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public TaskService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<CategoryDTO>> GetTaskCategoriesAsync()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.TaskCategories.Select(x => new CategoryDTO()
                {
                    Id = x.TaskCategoryId,
                    Name = x.Name,
                    Enum = (JobTasks.TaskCategory)x.TaskCategoryId
                }).ToListAsync();
            }
        }


        public async Task<IEnumerable<UsageStatusDTO>> GetUsageStatusAsync()
        {
            using(var db = _contextFactory.CreateDb())
            {
                return await db.UsageStatuses.Select(x => new UsageStatusDTO()
                {
                    Id = x.UsageStatusId,
                    Name = x.Name,
                    Enum = (JobTasks.UsageStatus)x.UsageStatusId
                }).ToListAsync();
            }
        }

        public async Task<IEnumerable<TaskDTO>> GetTasks()
        {
            using(var db = _contextFactory.CreateDb())
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
                        Name = x.TaskCategory.Name,
                        IsInternalCategory = x.TaskCategory.IsInternalOnly
                    },
                    LegacyCode = x.LegacyCode,
                    UsageStatus = new UsageStatusDTO()
                    {
                        Enum = (JobTasks.UsageStatus)x.UsageStatusId,
                        Id = x.UsageStatus.UsageStatusId,
                        Name = x.UsageStatus.Name
                    },
                    TaskId = x.JobTaskId,
                    ReportingType = new TaskReportingTypeDto()
                    {
                        Enum = (TaskReportingType)x.ReportingClassificationId,
                        Id = x.ReportingClassificationId,
                    }
                }).ToListAsync())
                .OrderBy(x => x.Category.Name)
                .ThenBy(x => x.LegacyCode)
                .ThenBy(x => x.Name);
            }
        }

        public async Task Upsert(TaskDTO task)
        {
            using(var db = _contextFactory.CreateDb())
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
                match.ReportingClassificationId = task.ReportingType.Id;
                db.SaveChanges();
            }
        }

    }
}
