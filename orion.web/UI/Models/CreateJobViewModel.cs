using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orion.Web.Clients;

namespace Orion.Web.Jobs
{
    public class CreateJobViewModel
    {
        public CoreJobDto Job { get; set; }
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
        public Dictionary<(CoreJobDto, JobModelDetail), bool> AllJobsWithAssociationStatus { get; set; }
    }
}
