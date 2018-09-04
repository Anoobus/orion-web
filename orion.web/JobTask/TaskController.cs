using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Employees;
using orion.web.Notifications;

namespace orion.web.JobsTasks
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;

        public TaskController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        // GET: Client/Create
        public ActionResult Create()
        {
            var vm = new TaskViewModel()
            {
                AllTaskCategories = Enum.GetValues(typeof(TaskCategoryId)).OfType<TaskCategoryId>().Select(x => new CategroyDTO()
                {
                    Id = (int)x,
                    Name = x.ToString()
                }),
                Task = new TaskDTO()
            };
            return View("Create", vm);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskViewModel client)
        {

            taskService.Post(new TaskDTO()
            {
                Description = client.Task.Description,
                Name = client.Task.Name,
                TaskCategory = (TaskCategoryId)client.SelectedCategory,
            });
            NotificationsController.AddNotification(this.User.SafeUserName(), $"{client.Task.Name} has been created");
            return RedirectToAction("Index", "Jobs");

        }

      
    }
}