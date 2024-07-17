using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Orion.Web.Clients
{
    public class ClientDTO : IEquatable<ClientDTO>
    {
        public string ClientName { get; set; }
        public int ClientId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClientDTO);
        }

        public bool Equals(ClientDTO other)
        {
            return other != null &&
                   ClientName == other.ClientName &&
                   ClientId == other.ClientId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientName, ClientId);
        }
    }
}
