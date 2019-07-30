using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Common;
using orion.web.Jobs;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using orion.web.Employees;

namespace orion.web.Expense
{

    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IJobService _jobsService;
        private readonly ISessionAdapter _sessionAdapter;
        private readonly IExpenseService _expenseService;

        public ExpenseController(IJobService jobsService, ISessionAdapter sessionAdapter, IExpenseService expenseService)
        {
            _jobsService = jobsService;
            _sessionAdapter = sessionAdapter;
            _expenseService = expenseService;
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
            var currentLocation = new FileInfo(this.GetType().Assembly.Location);
            var dir = Path.Combine(currentLocation.DirectoryName, "upload-data");
            return File(new FileStream(Path.Combine(dir, item.AttachmentId.Value.ToString()), FileMode.Open), "application/octet-stream", item.AttatchmentName);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AddExpenseViewModel vm)
        {
            var uploadId = new Guid?();
            if (vm.UploadFile != null)
            {
                uploadId = Guid.NewGuid();
                var currentLocation = new FileInfo(this.GetType().Assembly.Location);
                var thisItemId = uploadId.ToString();
                var dir = Path.Combine(currentLocation.DirectoryName, "upload-data");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
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
                RelatedJob = jobDetails,
                SaveDate = DateTimeOffset.UtcNow,
                WeekId = vm.WeekId
            });
            Notifications.NotificationsController.AddNotification(User.Identity.Name, $"{vm.UploadFile.Name} was uploaded.");
            return Redirect(vm.CancelUrl);
        }

    }

}

