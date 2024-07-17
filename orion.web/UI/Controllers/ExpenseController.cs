using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Orion.Web.Common;
using Orion.Web.Employees;
using Orion.Web.Jobs;

namespace Orion.Web.Expense
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IJobsRepository _jobsService;
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IExpenseService _expenseService;
        private readonly IConfiguration _config;
        private readonly IUploadLocationResolver _uploadLocationResolver;

        public ExpenseController(IJobsRepository jobsService, ISessionAdapter sessionAdapter, IExpenseService expenseService, IConfiguration config, IUploadLocationResolver uploadLocationResolver)
        {
            _jobsService = jobsService;
            _sessionAdapter = sessionAdapter;
            _expenseService = expenseService;
            _config = config;
            _uploadLocationResolver = uploadLocationResolver;
        }

        public async Task<ActionResult> Index(int weekid, string originUrl)
        {
            var jobs = await _jobsService.GetAsync(await _sessionAdapter.EmployeeIdAsync());
            var vm = new AddExpenseViewModel()
            {
                AvailableJobs = jobs.ToList(),
                ExpenseToSave = new ExpenseModel(),
                Week = WeekDTO.CreateForWeekId(weekid),
                CancelUrl = originUrl,
                WeekId = weekid,
            };
            return View("New", vm);
        }

        public async Task<ActionResult> Download(int expenseItemId)
        {
            var employee = await _sessionAdapter.EmployeeIdAsync();
            var item = await _expenseService.GetExpenseById(expenseItemId);
            var dir = _uploadLocationResolver.GetUploadPath();
            return File(new FileStream(Path.Combine(dir, item.AttachmentId.Value.ToString()), FileMode.Open), "application/octet-stream", item.AttatchmentName);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AddExpenseViewModel vm)
        {
            var uploadId = default(Guid?);
            if (vm.UploadFile != null)
            {
                uploadId = Guid.NewGuid();
                var thisItemId = uploadId.ToString();
                var dir = _uploadLocationResolver.GetUploadPath();
                using (var fs = new FileStream(Path.Combine(dir, thisItemId), FileMode.Create))
                {
                    await vm.UploadFile.CopyToAsync(fs);
                }
            }

            var jobDetails = await _jobsService.GetForJobId(vm.SelectedJobId);
            await _expenseService.SaveExpensesForEmployee(new ExpenseDTO()
            {
                AddtionalNotes = vm.ExpenseToSave.AddtionalNotes,
                Amount = vm.ExpenseToSave.Amount,
                AttachmentId = uploadId,
                AttatchmentName = vm.UploadFile != null ? $"{vm.ExpenseToSave.AttachmentName}-{vm.UploadFile.FileName}" : vm.ExpenseToSave.AttachmentName,
                Classification = vm.ExpenseToSave.Classification,
                EmployeeId = await _sessionAdapter.EmployeeIdAsync(),
                RelatedJob = jobDetails.CoreInfo,
                SaveDate = DateTimeOffset.UtcNow,
                WeekId = vm.WeekId
            });
            Notifications.NotificationsController.AddNotification(User.Identity.Name, $"{vm.UploadFile.Name} was uploaded.");
            return Redirect(vm.CancelUrl);
        }

        public async Task<ActionResult> Edit(int weekid, string originUrl, int expenseItemId)
        {
            var jobs = await _jobsService.GetAsync(await _sessionAdapter.EmployeeIdAsync());
            var expense = await _expenseService.GetExpenseById(expenseItemId);
            var vm = new EditExpenseViewModel()
            {
                AvailableJobs = jobs.ToList(),
                ExpenseToSave = new ExpenseModel()
                {
                    AddtionalNotes = expense.AddtionalNotes,
                    Amount = expense.Amount,
                    AttachmentName = expense.AttatchmentName,
                    Classification = expense.Classification,
                    RelatedJob = expense.RelatedJob,
                    SaveDate = expense.SaveDate
                },
                Week = WeekDTO.CreateForWeekId(weekid),
                CancelUrl = originUrl,
                WeekId = weekid,
                SelectedJobId = expense.RelatedJob.JobId,
                ExpenseItemId = expense.ExpenseItemId
            };
            return View("Edit", vm);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditExpenseViewModel vm, string postType)
        {
            if (postType == "Delete")
            {
                var deleted = await _expenseService.DeleteExpense(vm.ExpenseItemId);
                if (deleted != null)
                {
                    System.IO.File.Delete(Path.Combine(_uploadLocationResolver.GetUploadPath(), deleted.AttachmentId.Value.ToString()));
                }
            }
            else
            {
                var existing = await _expenseService.GetExpenseById(vm.ExpenseItemId);
                var job = await _jobsService.GetForJobId(vm.SelectedJobId);
                await _expenseService.SaveExpensesForEmployee(new ExpenseDTO()
                {
                    AddtionalNotes = vm.ExpenseToSave.AddtionalNotes,
                    Amount = vm.ExpenseToSave.Amount,
                    AttachmentId = vm.AttachmentId,
                    AttatchmentName = vm.ExpenseToSave.AttachmentName,
                    Classification = vm.ExpenseToSave.Classification,
                    EmployeeId = await _sessionAdapter.EmployeeIdAsync(),
                    RelatedJob = job.CoreInfo,
                    SaveDate = DateTimeOffset.UtcNow,
                    WeekId = vm.WeekId,
                    ExpenseItemId = vm.ExpenseItemId
                });
            }

            var jobDetails = await _jobsService.GetForJobId(vm.SelectedJobId);

            Notifications.NotificationsController.AddNotification(User.Identity.Name, $"{vm.ExpenseToSave.AttachmentName} was saved.");
            return Redirect(vm.CancelUrl);
        }
    }
}
