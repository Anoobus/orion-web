using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.Clients;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeEntries;
using System.Linq;


namespace orion.web.Jobs
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly IClientService clientService;
        private readonly IJobService jobService;
        private readonly IEmployeeService employeeService;
        private readonly ISiteService siteService;
        private readonly ITimeService timeService;
        private readonly ISessionAdapter sessionAdapter;

        public JobsController(IClientService clientService, IJobService jobService, IEmployeeService employeeService, ISiteService siteService, ITimeService timeService, ISessionAdapter sessionAdapter)
        {
            this.clientService = clientService;
            this.jobService = jobService;
            this.employeeService = employeeService;
            this.siteService = siteService;
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
            var jobs = await jobService.GetAsync();
            var myJobs = await jobService.GetAsync(await sessionAdapter.EmployeeIdAsync());
            var vm = new JobListViewModel()
            {
                AllJobsWithAssociationStatus = jobs.OrderBy(x => x.FullJobCode).ToDictionary(x => x, x => myJobs.Any(z => z.JobId == x.JobId)),
                HeaderHelp = new JobDTO()
            };
            return View("JobList", vm);
        }

        public async System.Threading.Tasks.Task<ActionResult> AddJobForCurrentUser(int id)
        {
            var me = await employeeService.GetSingleEmployeeAsync(this.User.Identity.Name);
            me.AssignJobs.Add(id);
            employeeService.Save(me);
            var employeeName = this.User.Identity.Name;
            NotificationsController.AddNotification(this.User.Identity.Name, "Job Added");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> RemoveJobForCurrentUser(int id)
        {

            var me = await employeeService.GetSingleEmployeeAsync(this.User.Identity.Name);
            if(me.AssignJobs.Contains(id))
            {
                me.AssignJobs.Remove(id);
            }
            employeeService.Save(me);

            NotificationsController.AddNotification(this.User.SafeUserName(), $"Removed from my jobs");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(int id)
        {
            var job = (await jobService.GetAsync()).SingleOrDefault(x => x.JobId == id);
            return View("Details", job);
        }


        [Authorize(Roles = UserRoleName.Admin)]
        public ActionResult Create()
        {
            var vm = new CreateJobViewModel()
            {
                Job = new JobDTO(),
                AvailableClients = clientService.Get(),
                AvailableSites = siteService.Get()
            };
            return View("Create", vm);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        public ActionResult Create(CreateJobViewModel aLongMotherFuckingNonCollidingName)
        {
            // TODO: Add insert logic here
            var clients = clientService.Get();
            var sites = siteService.Get();
            var mappedJob = aLongMotherFuckingNonCollidingName.Job;
            mappedJob.Client = clients.FirstOrDefault(x => x.ClientId == aLongMotherFuckingNonCollidingName.SelectedClientId);
            mappedJob.Site = sites.FirstOrDefault(x => x.SiteID == aLongMotherFuckingNonCollidingName.SelectedSiteId);
            jobService.Post(mappedJob);
            NotificationsController.AddNotification(this.User.SafeUserName(), $"Sucessfully created {aLongMotherFuckingNonCollidingName.Job.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleName.Admin)]
        public async System.Threading.Tasks.Task<ActionResult> Edit(JobDTO job)
        {
            var jobSaved = (await jobService.GetAsync()).Single(x => x.JobId == job.JobId);
            jobSaved.JobName = job.JobName;
            jobSaved.TargetHours = job.TargetHours;
            jobService.Put((jobSaved));
            NotificationsController.AddNotification(this.User.SafeUserName(), $"Updated {jobSaved.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }

    }
}
