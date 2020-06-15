using Microsoft.AspNetCore.Mvc.ModelBinding;
using orion.web.Clients;
using System.Collections.Generic;

namespace orion.web.Jobs
{
    public class CreateJobViewModel
    {
        public JobDTO Job { get; set; }
        public int SelectedSiteId { get; set; }
        public int SelectedClientId { get; set; }
        public int SelectedJobStatusId { get; set; }
        public int SelectedProjectManagerEmployeeId { get; set; }
        [BindNever]
        public IEnumerable<SiteDTO> AvailableSites { get; set; }
        [BindNever]
        public IEnumerable<ClientDTO> AvailableClients { get; set; }
        [BindNever]
        public IEnumerable<JobStatusDTO> AvailableJobStatus { get; set; }
        [BindNever]
        public IEnumerable<ProjectManagerDTO> AvailableProjectManagers { get; set; }
    }

    public class JobListViewModel
    {
        public JobDTO HeaderHelp { get; set; }
        public Dictionary<JobDTO, bool> AllJobsWithAssociationStatus { get; set; }
    }
}
