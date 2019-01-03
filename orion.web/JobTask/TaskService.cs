using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using orion.web.DataAccess.EF;

namespace orion.web.JobsTasks
{
    public interface ITaskService
    {
        IEnumerable<TaskDTO> GetTasks();
        Task<IEnumerable<CategroyDTO>> GetTaskCategoriesAsync();
        void Post(TaskDTO newTask);
    }

    public class TaskService :  ITaskService
    {
        private readonly OrionDbContext db;

        public TaskService(OrionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CategroyDTO>> GetTaskCategoriesAsync()
        {
            return await db.TaskCategories.Select(x => new CategroyDTO() {
                 Id = x.TaskCategoryId,
                  Name = x.Name
            }).ToListAsync();
        }

        public IEnumerable<TaskDTO> GetTasks()
        {
            return db.JobTasks.Select(x => new TaskDTO()
            {
                Description = x.Description,
                Name = x.ShortName,
                TaskCategoryName = x.TaskCategory.Name,
                TaskId = x.JobTaskId,
            }).ToList();
        }
       
        public void Post(TaskDTO newTask)
        {
            var cat = db.TaskCategories.Single(x => x.Name == newTask.TaskCategoryName);
            var jt = new JobTask()
            {
                Description = newTask.Description,
                ShortName = newTask.Name,
                TaskCategoryId = cat.TaskCategoryId,
            };
            db.JobTasks.Add(jt);
            db.SaveChanges();
        }

    }
}
