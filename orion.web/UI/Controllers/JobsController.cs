using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Clients;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeEntries;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Jobs
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly IClientsRepository clientRepository;
        private readonly IJobService jobService;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISitesRepository siteRepository;
        private readonly ITimeService timeService;
        private readonly ISessionAdapter sessionAdapter;

        public JobsController(IClientsRepository clientRepository, IJobService jobService, IEmployeeRepository employeeRepository, ISitesRepository siteRepository, ITimeService timeService, ISessionAdapter sessionAdapter)
        {
            this.clientRepository = clientRepository;
            this.jobService = jobService;
            this.employeeRepository = employeeRepository;
            this.siteRepository = siteRepository;
            this.timeService = timeService;
            this.sessionAdapter = sessionAdapter;
        }

        public ActionResult Index()
        {
            if(User.IsInRole(UserRoleName.Admin))
            {
                return View("Index");
            }
            else
            {
                return RedirectToAction(nameof(List));
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> List()
        {
            var isAdmin = User.IsInRole(UserRoleName.Admin);
            var jobs = await jobService.GetAsync();
            if(!isAdmin)
            {
                jobs = jobs.Where(x => x.JobStatusDTO.Enum == JobStatus.Enabled).ToList();
            }
            var myJobs = await jobService.GetAsync(await sessionAdapter.EmployeeIdAsync());
            var vm = new JobListViewModel()
            {
                AllJobsWithAssociationStatus = jobs.OrderBy(x => x.JobCode).ToDictionary(x => x, x => myJobs.Any(z => z.JobId == x.JobId)),
                HeaderHelp = new JobDTO()
            };
            return View("ListJobs", vm);
        }

        public async System.Threading.Tasks.Task<ActionResult> AddJobForCurrentUser(int id)
        {
            var me = await employeeRepository.GetSingleEmployeeAsync(User.Identity.Name);
            me.AssignJobs.Add(id);
            employeeRepository.Save(me);
            var employeeName = User.Identity.Name;
            NotificationsController.AddNotification(User.Identity.Name, "Job Added");
            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public async Task<ActionResult> CloseJob(int id)
        {
            var job = await jobService.GetForJobId(id);
            job.JobStatusDTO.Id = (int)JobStatus.Archived;
            await jobService.PutAsync(job);
            NotificationsController.AddNotification(User.Identity.Name, $"Job {job.FullJobCodeWithName} has been closed");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> RemoveJobForCurrentUser(int id)
        {

            var me = await employeeRepository.GetSingleEmployeeAsync(User.Identity.Name);
            if(me.AssignJobs.Contains(id))
            {
                me.AssignJobs.Remove(id);
            }
            employeeRepository.Save(me);

            NotificationsController.AddNotification(User.SafeUserName(), $"Removed from my jobs");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(int id)
        {
            var job = (await jobService.GetAsync()).SingleOrDefault(x => x.JobId == id);
            var jobStatus = await jobService.GetUsageStatusAsync();
            return View("EditJob", new EditJobViewModel()
            {
                Job = job,
                SelectedJobStatusId = job.JobStatusDTO.Id,
                SelectedProjectManagerEmployeeId = job.ProjectManager.EmployeeId,
                AvailableJobStatus = jobStatus.ToList(),
                AvailableProjectManagers = (await employeeRepository.GetAllEmployees()).Select(x => new ProjectManagerDTO()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = $"{x.Last}, {x.First}"
                })
            });
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public async Task<ActionResult> Create()
        {
            var vm = new CreateJobViewModel()
            {
                Job = new JobDTO(),
                AvailableClients = await clientRepository.GetAllClients(),
                AvailableSites = await siteRepository.GetAll(),
                AvailableJobStatus = await jobService.GetUsageStatusAsync(),
                AvailableProjectManagers = (await employeeRepository.GetAllEmployees()).Select(x => new ProjectManagerDTO()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = $"{x.Last}, {x.First}"
                })
            };
            return View("CreateJob", vm);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        public async Task<ActionResult> Create(CreateJobViewModel jobToCreate)
        {
            var clients = await clientRepository.GetAllClients();
            var sites = await siteRepository.GetAll();
            var mappedJob = jobToCreate.Job;
            mappedJob.Client = clients.FirstOrDefault(x => x.ClientId == jobToCreate.SelectedClientId);
            mappedJob.Site = sites.FirstOrDefault(x => x.SiteID == jobToCreate.SelectedSiteId);
            mappedJob.JobStatusDTO = new JobStatusDTO() { Id = jobToCreate.SelectedJobStatusId };
            mappedJob.ProjectManager = new ProjectManagerDTO() { EmployeeId = jobToCreate.SelectedProjectManagerEmployeeId };
            jobService.Post(mappedJob);
            NotificationsController.AddNotification(User.SafeUserName(), $"Sucessfully created {jobToCreate.Job.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleName.Admin)]
        public async System.Threading.Tasks.Task<ActionResult> Edit(EditJobViewModel model)
        {
            var jobSaved = (await jobService.GetAsync()).Single(x => x.JobId == model.Job.JobId);
            jobSaved.JobName = model.Job.JobName;
            jobSaved.TargetHours = model.Job.TargetHours;
            jobSaved.JobStatusDTO.Id = model.SelectedJobStatusId;
            jobSaved.ProjectManager.EmployeeId = model.SelectedProjectManagerEmployeeId;
            await jobService.PutAsync((jobSaved));
            NotificationsController.AddNotification(User.SafeUserName(), $"Updated {jobSaved.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }

    }
}
