using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Clients
{
    public class ClientModel
    {
        [DisplayName("Client")]
        [Required(AllowEmptyStrings = false)]
        public string ClientName { get; set; }
        public int ClientId { get; set; }
    }
}
