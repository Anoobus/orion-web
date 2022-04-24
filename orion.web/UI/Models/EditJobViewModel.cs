using Microsoft.AspNetCore.Mvc.ModelBinding;
using orion.web.Clients;
using orion.web.UI.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace orion.web.Jobs
{
    public class EditJobViewModel
    {

        public CoreJobDto Job { get; set; }
        public int SelectedProjectManagerEmployeeId { get; set; }
        [DisplayName("Status")]
        public int SelectedJobStatusId { get; set; }
        public int SelectedSiteId { get; set; }
        public int SelectedClientId { get; set; }

        [BindNever]
        public IEnumerable<JobStatusDTO> AvailableJobStatus { get; set; }
        [BindNever]
        public IEnumerable<ProjectManagerDTO> AvailableProjectManagers { get; set; }

        [BindNever]
        public IEnumerable<SiteModel> AvailableSites { get; set; }
        [BindNever]
        public IEnumerable<ClientModel> AvailableClients { get; set; }
    }
}
