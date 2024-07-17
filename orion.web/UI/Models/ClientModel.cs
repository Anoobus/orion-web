using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Orion.Web.UI.Models
{
    public class ClientModel
    {
        [DisplayName("Client Name")]
        [Required(AllowEmptyStrings = false)]
        public string ClientName { get; set; }
        [HiddenInput]
        public int ClientId { get; set; }
    }
}
