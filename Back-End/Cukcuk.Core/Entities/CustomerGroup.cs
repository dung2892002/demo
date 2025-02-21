using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class CustomerGroup
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
