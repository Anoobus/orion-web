using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.BLL.Jobs;
using orion.web.Clients;
using orion.web.Common;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.TimeEntries;
using orion.web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Jobs
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly IClientsRepository clientRepository;
        private readonly IJobsRepository _jobsRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISitesRepository siteRepository;
        private readonly ITimeService timeService;
        private readonly ISessionAdapter sessionAdapter;
        private readonly IMapper _mapper;

        public JobsController(IClientsRepository clientRepository,
            IJobsRepository jobsRepository,
            IEmployeeRepository employeeRepository,
            ISitesRepository siteRepository,
            ITimeService timeService,
            ISessionAdapter sessionAdapter,
            IMapper mapper)
        {
            this.clientRepository = clientRepository;
            this._jobsRepository = jobsRepository;
            this._employeeRepository = employeeRepository;
            this.siteRepository = siteRepository;
            this.timeService = timeService;
            this.sessionAdapter = sessionAdapter;
            _mapper = mapper;
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
            var jobs = await _jobsRepository.GetAsync();
            if(!isAdmin)
            {
                jobs = jobs.Where(x => x.JobStatusId == JobStatus.Enabled).ToList();
            }
            var myJobs = await _jobsRepository.GetAsync(await sessionAdapter.EmployeeIdAsync());
            var availableClients = await clientRepository.GetAllClients();
            var availableSites = await siteRepository.GetAll();
            var allEmployees = await _employeeRepository.GetAllEmployees();
            var models = jobs.OrderBy(x => x.JobCode).Select(job =>
            {
                var details = new JobModelDetail
                {
                    Client = _mapper.Map<ClientModel>(availableClients.SingleOrDefault(z => z.ClientId == job.ClientId)),
                    Site = _mapper.Map<SiteModel>(availableSites.SingleOrDefault(z => z.SiteID == job.SiteId)),
                    ProjectManager = _mapper.Map<ProjectManagerModel>(allEmployees.SingleOrDefault(z => z.EmployeeId == job.ProjectManagerEmployeeId))
                };
                return (job, details);
            }).ToList();
            var vm = new JobListViewModel()
            {
                AllJobsWithAssociationStatus = models.OrderBy(x => x.job.JobCode)
                                                   .ToDictionary(x => x, x => myJobs.Any(z => z.JobId == x.job.JobId)),
            };
            return View("ListJobs", vm);
        }

        public async System.Threading.Tasks.Task<ActionResult> AddJobForCurrentUser(int id)
        {
            var me = await _employeeRepository.GetSingleEmployeeAsync(User.Identity.Name);
            me.AssignJobs.Add(id);
            _employeeRepository.Save(me);
            var employeeName = User.Identity.Name;
            NotificationsController.AddNotification(User.Identity.Name, "Job Added");
            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public async Task<ActionResult> CloseJob(int id)
        {
            var job = await _jobsRepository.GetForJobId(id);
            job.JobStatusId = JobStatus.Archived;
            await _jobsRepository.Update(job);
            NotificationsController.AddNotification(User.Identity.Name, $"Job {job.FullJobCodeWithName} has been closed");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> RemoveJobForCurrentUser(int id)
        {

            var me = await _employeeRepository.GetSingleEmployeeAsync(User.Identity.Name);
            if(me.AssignJobs.Contains(id))
            {
                me.AssignJobs.Remove(id);
            }
            _employeeRepository.Save(me);

            NotificationsController.AddNotification(User.SafeUserName(), $"Removed from my jobs");
            return RedirectToAction(nameof(List));
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(int id)
        {
            var availableClients = await clientRepository.GetAllClients();
            var availableSites = await siteRepository.GetAll();
            var job = (await _jobsRepository.GetAsync()).SingleOrDefault(x => x.JobId == id);
            var jobStatus = await _jobsRepository.GetUsageStatusAsync();
            return View("EditJob", new EditJobViewModel()
            {
                Job = job,
                SelectedJobStatusId = (int)job.JobStatusId,
                SelectedProjectManagerEmployeeId = job.ProjectManagerEmployeeId,
                AvailableJobStatus = jobStatus.ToList(),
                AvailableProjectManagers = (await _employeeRepository.GetAllEmployees()).Select(x => new ProjectManagerDTO()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = $"{x.Last}, {x.First}"
                }),
                AvailableClients = _mapper.Map<IEnumerable<ClientModel>>(availableClients),
                AvailableSites = _mapper.Map<IEnumerable<SiteModel>>(availableSites),
                SelectedClientId = job.ClientId,
                SelectedSiteId = job.SiteId
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
                AvailableJobStatus = await _jobsRepository.GetUsageStatusAsync(),
                AvailableProjectManagers = (await _employeeRepository.GetAllEmployees()).Select(x => new ProjectManagerDTO()
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
            var createDto = new CreateJobDto()
            {
                ClientId = jobToCreate.SelectedClientId,
                JobCode = jobToCreate.Job.JobCode,
                JobName = jobToCreate.Job.JobName,
                JobStatusId = (JobStatus)jobToCreate.SelectedJobStatusId,
                ProjectManagerEmployeeId = jobToCreate.SelectedProjectManagerEmployeeId,
                SiteId = jobToCreate.SelectedSiteId,
                TargetHours = jobToCreate.Job.TargetHours,
            };

            await _jobsRepository.Create(createDto);
            NotificationsController.AddNotification(User.SafeUserName(), $"Sucessfully created {jobToCreate.Job.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleName.Admin)]
        public async System.Threading.Tasks.Task<ActionResult> Edit(EditJobViewModel model)
        {
            var jobSaved = (await _jobsRepository.GetAsync()).Single(x => x.JobId == model.Job.JobId);
            jobSaved.JobName = model.Job.JobName;
            jobSaved.TargetHours = model.Job.TargetHours;
            jobSaved.JobStatusId = (JobStatus)model.SelectedJobStatusId;
            jobSaved.ProjectManagerEmployeeId = model.SelectedProjectManagerEmployeeId;
            jobSaved.ClientId = model.SelectedClientId;
            jobSaved.SiteId = model.SelectedSiteId;
            jobSaved.JobCode = model.Job.JobCode;
            await _jobsRepository.Update(jobSaved);
            NotificationsController.AddNotification(User.SafeUserName(), $"Updated {jobSaved.FullJobCodeWithName}");
            return RedirectToAction(nameof(Index));
        }

    }
}
