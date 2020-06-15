using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace orion.web.UI.Models
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
