using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class EmployeeFolder
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public EmployeeFolder? Parent { get; set; }

        public Employee? Employee { get; set; }

        [JsonIgnore]
        public List<EmployeeFolder> Children { get; set; } = new List<EmployeeFolder>();
    }
}
