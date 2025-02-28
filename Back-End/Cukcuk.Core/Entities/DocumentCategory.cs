using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class DocumentCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;


        [JsonIgnore]
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}
