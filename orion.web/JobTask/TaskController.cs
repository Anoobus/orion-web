using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Create()
        {
            var cats = await taskService.GetTaskCategoriesAsync();
            var vm = new TaskViewModel()
            {
                AllTaskCategories =cats,
                Task = new TaskDTO()
            };
            return View("Create", vm);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskViewModel submittedTask)
        {
            var cats = await taskService.GetTaskCategoriesAsync();
            var catName = cats.FirstOrDefault(x => x.Id == submittedTask.SelectedCategory).Name;
            taskService.Post(new TaskDTO()
            {
                Description = submittedTask.Task.Description,
                Name = submittedTask.Task.Name,
                TaskCategoryName = catName,
            });
            NotificationsController.AddNotification(this.User.SafeUserName(), $"{submittedTask.Task.Name} has been created");
            return RedirectToAction("Index", "Jobs");

        }

      
    }
}