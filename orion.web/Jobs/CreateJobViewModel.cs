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
        [BindNever]
        public IEnumerable<SiteDTO> AvailableSites { get; set; }
        [BindNever]
        public IEnumerable<ClientDTO> AvailableClients { get; set; }
    }

    public class JobListViewModel
    {
        public JobDTO HeaderHelp { get; set; }
        public Dictionary<JobDTO, bool> AllJobsWithAssociationStatus { get; set; }
    }

   
  
  
  

   
}
