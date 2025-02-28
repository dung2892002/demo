using Cukcuk.Core.Enum;
using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DocumentType Type { get; set; }

        public Guid? ParentId { get; set; }
        [JsonIgnore]
        public Document? Parent { get; set; }

        public List<Document> Children { get; set; } = new List<Document>();

        public Guid? CategoryId { get; set; }
        public DocumentCategory? Category { get; set; }

    }
}
