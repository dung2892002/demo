using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class Position
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string PositionCode { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = new DateTime();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; } = new DateTime();
        public string ModifiedBy { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
