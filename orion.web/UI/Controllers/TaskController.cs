using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.BLL.JobTasks;
using Orion.Web.Employees;
using Orion.Web.Notifications;

namespace Orion.Web.JobsTasks
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
            TaskViewModel vm = await GetViewModel();
            vm.IsInCreateModel = true;
            return View("TaskDetail", vm);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var taskToEdit = (await taskService.GetTasks()).Single(x => x.TaskId == id);
            TaskViewModel vm = await GetViewModel(taskToEdit);
            return View("TaskDetail", vm);
        }

        private async Task<TaskViewModel> GetViewModel(TaskDTO existingTask = null)
        {
            var cats = await taskService.GetTaskCategoriesAsync();
            var usageStats = await taskService.GetUsageStatusAsync();
            var reportingTypes = Enum.GetValues(typeof(TaskReportingType))
                                     .OfType<TaskReportingType>()
                                     .Select(x => new TaskReportingTypeDto()
                                     {
                                         Enum = x,
                                         Id = (int)x,
                                     }).ToList();

            var vm = new TaskViewModel()
            {
                AllTaskCategories = cats,
                AllUsageStatusOptions = usageStats.Where(x => x.Enum != JobTasks.UsageStatus.Unkown),
                AllTaskReportingTypes = reportingTypes,
                IsInCreateModel = existingTask == null,
                SelectedCategory = existingTask?.Category?.Id ?? 0,
                SelectedUsageStatus = existingTask?.UsageStatus?.Id ?? 0,
                SelectedTaskReportingType = existingTask?.ReportingType?.Id ?? 0,
                Task = existingTask ?? new TaskDTO()
            };
            return vm;
        }

        [HttpGet]
        public async Task<ActionResult> List()
        {
            var allTasks = (await taskService.GetTasks()).ToArray();
            var cats = await taskService.GetTaskCategoriesAsync();
            var vm = new TaskListViewModel()
            {
                Tasks = allTasks,
                HeaderHelp = allTasks.FirstOrDefault()
            };
            return View("TaskList", vm);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(TaskViewModel submittedTask)
        {
            var vm = await GetViewModel();
            var category = vm.AllTaskCategories.Single(x => x.Id == submittedTask.SelectedCategory);
            var usageStatus = vm.AllUsageStatusOptions.Single(x => x.Id == submittedTask.SelectedUsageStatus);
            var reportingType = vm.AllTaskReportingTypes.Single(x => x.Id == submittedTask.SelectedTaskReportingType);

            var allTasks = (await taskService.GetTasks()).ToList();
            var isBadCreateLegacyCode = submittedTask.IsInCreateModel
                && allTasks.Any(x => x.LegacyCode == submittedTask.Task.LegacyCode);
            var isBadUpdateLegacyCode = !submittedTask.IsInCreateModel
                && allTasks.Any(x => x.LegacyCode == submittedTask.Task.LegacyCode && x.TaskId != submittedTask.Task.TaskId);

            if (isBadCreateLegacyCode || isBadUpdateLegacyCode)
            {
                vm.SelectedCategory = submittedTask.SelectedCategory;
                vm.SelectedUsageStatus = submittedTask.SelectedUsageStatus;
                vm.SelectedTaskReportingType = submittedTask.SelectedTaskReportingType;
                vm.Task.LegacyCode = submittedTask.Task.LegacyCode;
                vm.Task.Name = submittedTask.Task.Name;
                ModelState.Clear();
                ModelState.AddModelError("", $"The supplied task code ({submittedTask.Task.LegacyCode}) is already in use");
                return View("TaskDetail", vm);
            }
            else
            {
                await taskService.Upsert(new TaskDTO()
                {
                    Description = submittedTask.Task.Description,
                    Name = submittedTask.Task.Name,
                    Category = category,
                    LegacyCode = submittedTask.Task.LegacyCode,
                    UsageStatus = usageStatus,
                    TaskId = submittedTask.Task.TaskId,
                    ReportingType = reportingType
                });
                NotificationsController.AddNotification(User.SafeUserName(), $"{submittedTask.Task.Name} has been saved");
                return RedirectToAction("List", "Task");
            }
        }
    }
}
