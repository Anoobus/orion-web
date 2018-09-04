using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace orion.web.Clients
{
    public class ClientDTO
    {
        [DisplayName("Client")]
        [Required(AllowEmptyStrings = false)]
        public string ClientName { get; set; }
        public string FullName => $"{ClientCode}-{ClientName}";
        public int ClientId { get; set; }
        [DisplayName("Code")]
        [RegularExpression("[0-9]{4}", ErrorMessage = "Client code needs to be in the format ####")]
        [StringLength(4,MinimumLength =4)]
        public string ClientCode { get; set; }
    }
}
