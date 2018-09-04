using System.Collections.Generic;
using System.Linq;
using orion.web.DataAccess.EF;

namespace orion.web.JobsTasks
{
    public interface ITaskService
    {
        IEnumerable<TaskDTO> Get();
        void Post(TaskDTO newTask);
    }

    public class TaskService :  ITaskService
    {
        private readonly OrionDbContext db;

        public TaskService(OrionDbContext db)
        {
            this.db = db;
        }
        public IEnumerable<TaskDTO> Get()
        {
            return db.JobTasks.Select(x => new TaskDTO()
            {
                Description = x.Description,
                Name = x.ShortName,
                TaskCategory = (TaskCategoryId)x.TaskCategoryId,
                TaskId = x.JobTaskId,
            }).ToList();

        }
        public void Post(TaskDTO newTask)
        {
            var jt = new JobTask()
            {
                Description = newTask.Description,
                ShortName = newTask.Name,
                TaskCategoryId = (int)newTask.TaskCategory,
            };
            db.JobTasks.Add(jt);
            db.SaveChanges();
        }

    }
}
