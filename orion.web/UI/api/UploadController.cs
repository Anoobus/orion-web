using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using orion.web.Clients;
using orion.web.Employees;
using orion.web.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.UI.api
{
    public class JobUploadResults
    {
        public Dictionary<string, int> NewClients { get; set; }
        public Dictionary<string, int> NewSites { get; set; }
        public IEnumerable<JobDTO> CreatedJobs { get; set; }
        public IEnumerable<JobDTO> UpdatedJobs { get; set; }
        public IEnumerable<JobDTO> SkippedEntriesBecuaseNoChange { get; set; }
    }
    public class JobUploadModel
    {
        public string JobCode { get; set; }
        public string ClientName { get; set; }
        public string SiteName { get; set; }
        public string JobName { get; set; }
        public int? ProjectManager { get; set; }
    }

    [Authorize]
    [Route("api/v1/uploads")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly ISitesRepository _sitesRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IJobsRepository _jobsRepository;
        private readonly IMapper _mapper;

        public UploadsController(ISitesRepository sitesRepository, IClientsRepository clientsRepository, IJobsRepository jobsRepository, IMapper mapper)
        {
            _sitesRepository = sitesRepository;
            _clientsRepository = clientsRepository;
            _jobsRepository = jobsRepository;
            _mapper = mapper;
        }

        [HttpPut("bulk-jobs")]
        public async Task<ActionResult<IEnumerable<JobDTO>>> SaveBulkJobs([FromBody] IEnumerable<JobUploadModel> jobs)
        {
            var res = new JobUploadResults()
            {
                NewClients = new Dictionary<string, int>(),
                NewSites = new Dictionary<string, int>(),

            };
            try
            {
                var allSites = await _sitesRepository.GetAll();
                var sitesBySiteName = await BulkSaveSites(allSites, jobs, newSite => res.NewSites.TryAdd(newSite.SiteName, newSite.SiteID));

                var allClients = await _clientsRepository.GetAllClients();
                var clientsByClientName = await BulkSaveClients(allClients, jobs, newClient => res.NewClients.TryAdd(newClient.ClientName, newClient.ClientId));

                var allJobs = await _jobsRepository.GetAsync();

                var newJobs = new List<JobDTO>();
                var updatedJobs = new List<JobDTO>();
                var skippedEntry = new List<JobDTO>();
                foreach(var rec in jobs)
                {
                    var isBrandNewJob = false;
                    var temp = rec;
                    var matchedJob = await SaveForMatchingField(allJobs,
                                               matchCriteria: (existingJob, newJob) => {
                                                   var wtf = existingJob.JobCode.Replace("-","").Equals(newJob.JobCode.Replace("-",""), StringComparison.InvariantCultureIgnoreCase);
                                                   return wtf;
                                                },
                                               factory: async (newJob) =>
                                               {
                                                   var created = await _jobsRepository.Create(new BLL.Jobs.CreateJobDto()
                                                   {
                                                       ClientId = clientsByClientName[newJob.ClientName.Trim()],
                                                       JobCode = newJob.JobCode,
                                                       JobName = newJob.JobName,
                                                       JobStatusId = JobStatus.Enabled,
                                                       SiteId = sitesBySiteName[newJob.SiteName.Trim()],
                                                       ProjectManagerEmployeeId = newJob.ProjectManager ?? 1
                                                   });
                                                   isBrandNewJob = true;

                                                   return created;
                                               },
                                               field: temp);

                    var nameChanged = matchedJob.JobName != rec.JobName;
                    var clientChanged = matchedJob.ClientId != clientsByClientName[rec.ClientName.Trim()];
                    var siteChanged = matchedJob.SiteId != sitesBySiteName[rec.SiteName.Trim()];
                    if(isBrandNewJob)
                    {
                        newJobs.Add(matchedJob);
                    }
                    else if(nameChanged || clientChanged || siteChanged)
                    {
                        matchedJob.JobName = rec.JobName;
                        matchedJob.ClientId = clientsByClientName[rec.ClientName.Trim()];
                        matchedJob.SiteId = sitesBySiteName[rec.SiteName.Trim()];
                        await _jobsRepository.Update(matchedJob);
                        updatedJobs.Add(matchedJob);
                    }
                    else
                    {
                        skippedEntry.Add(matchedJob);
                    }
                }

                res.UpdatedJobs = updatedJobs;
                res.CreatedJobs = newJobs;
                res.SkippedEntriesBecuaseNoChange = skippedEntry;
            }
            catch(Exception e)
            {
                return new ObjectResult(new
                {
                    Error = new ProblemDetails()
                    {
                        Title = e.Message,
                        Detail = e.ToString(),
                        Instance = this.HttpContext.Request.GetDisplayUrl(),
                        Status = StatusCodes.Status500InternalServerError,
                        Type = "Unhandled Error"
                    },
                    Processed = res
                });

            }


            return Ok(res);
        }

        private async Task<Dictionary<string, int>> BulkSaveSites(IEnumerable<SiteDTO> allSites, IEnumerable<JobUploadModel> recs, Action<SiteDTO> onNewSiteCreated)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var sitesBySiteName = new Dictionary<string, int >(comparer);
            foreach(var rec in recs.Select(z => z.SiteName.Trim()).Distinct())
            {
                var matchedSite = await SaveForMatchingField(allSites,
                                                               matchCriteria: (site, field) => site.SiteName.Trim().Equals(field, StringComparison.InvariantCultureIgnoreCase),
                                                               factory: async (field) =>
                                                               {
                                                                   var site = new SiteDTO()
                                                                   {
                                                                       SiteID = await _sitesRepository.Create(new SiteDTO() { SiteName = field }),
                                                                       SiteName = field
                                                                   };
                                                                   onNewSiteCreated(site);
                                                                   return site;
                                                               },
                                                               field: rec);

                sitesBySiteName.TryAdd(matchedSite.SiteName.Trim(), matchedSite.SiteID);
            }
            return sitesBySiteName;
        }

        private async Task<Dictionary<string, int>> BulkSaveClients(IEnumerable<ClientDTO> allClients, IEnumerable<JobUploadModel> recs, Action<ClientDTO> onNewClientCreated)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var clientsByClientName = new Dictionary<string, int>(comparer);
            foreach(var rec in recs.Select(z => z.ClientName.Trim()).Distinct())
            {
                var matchedClient = await SaveForMatchingField(allClients,
                                                                 matchCriteria: (client, field) => client.ClientName.Trim().Equals(field, StringComparison.InvariantCultureIgnoreCase),
                                                                 factory: async (field) =>
                                                                 {
                                                                     var client = await _clientsRepository.Save(new ClientDTO()
                                                                     {
                                                                         ClientName = field
                                                                     });
                                                                     onNewClientCreated(client);
                                                                     return client;
                                                                 },
                                                                 field: rec);

                clientsByClientName.TryAdd(matchedClient.ClientName.Trim(), matchedClient.ClientId);
            }
            return clientsByClientName;
        }

        private async Task<T> SaveForMatchingField<T,U>(
            IEnumerable<T> currentListing,
            Func<T, U, bool> matchCriteria,
            Func<U, Task<T>> factory,
            U field)
        {
            var matchedSites = currentListing.Where(x => matchCriteria(x, field)).ToList();
            var matchedSite = matchedSites.FirstOrDefault();
            if(matchedSites.Count() > 1)
            {
                Console.WriteLine($"Multiple matching entries: {JsonConvert.SerializeObject(matchedSites)}");
            }

            if(matchedSite == null)
            {
                matchedSite = await factory(field);
            }
            return matchedSite;
        }
    }
}
