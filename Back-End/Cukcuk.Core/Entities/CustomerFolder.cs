using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class CustomerFolder
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public CustomerFolder? Parent { get; set; }

        public Customer? Customer { get; set; }

        [JsonIgnore]
        public List<CustomerFolder> Children { get; set; } = new List<CustomerFolder>();
    }
}
