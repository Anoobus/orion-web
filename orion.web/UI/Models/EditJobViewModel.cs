using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orion.Web.Clients;
using Orion.Web.UI.Models;

namespace Orion.Web.Jobs
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
