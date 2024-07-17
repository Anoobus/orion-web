using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orion.Web.BLL.ScheduledTasks;
using Orion.Web.DataAccess;
using Orion.Web.DataAccess.EF;
using Orion.Web.Util.IoC;

namespace Orion.Web.Employees
{
    public interface IScheduledTaskRepo
    {
        Task<ScheduleTask> GetTaskByName(string taskName);
        Task<ScheduleTask> CreateNewScheduledTask(NewScheduledTask task);
        Task RecordTaskCompletion(int taskId);
        Task<DateTimeOffset?> GetLastRunTime(int taskId);
    }

    public class ScheduledTaskRepo : IScheduledTaskRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public ScheduledTaskRepo(IContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<ScheduleTask> CreateNewScheduledTask(NewScheduledTask task)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var newTask = _mapper.Map<ScheduleTask>(task);
                db.ScheduleTasks.Add(newTask);
                await db.SaveChangesAsync();
                return newTask;
            }
        }

        public async Task RecordTaskCompletion(int taskId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                db.ScheduleTaskRunLogs.Add(new ScheduleTaskRunLog()
                {
                    ScheduleTaskId = taskId,
                    TaskCompletionDate = DateTimeOffset.Now,
                });

                await db.SaveChangesAsync();
            }
        }

        public async Task<DateTimeOffset?> GetLastRunTime(int taskId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var lastRun = await db.ScheduleTaskRunLogs.Where(x => x.ScheduleTaskId == taskId)
                    .OrderByDescending(x => x.TaskCompletionDate).FirstOrDefaultAsync();
                return lastRun?.TaskCompletionDate;
            }
        }

        public async Task<ScheduleTask> GetTaskByName(string taskName)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.ScheduleTasks.SingleOrDefaultAsync(x => x.TaskName == taskName);
            }
        }
    }
}
