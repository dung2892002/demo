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
        public string FolderPath { get; set; } = string.Empty;

        public Guid? ParentId { get; set; }
        [JsonIgnore]
        public Document? Parent { get; set; }

        [JsonIgnore]
        public List<Document> Children { get; set; } = new List<Document>();

        public Guid? CategoryId { get; set; }
        public DocumentCategory? Category { get; set; }

        public List<DocumentBlock> DocumentBlocks { get; set; } = new List<DocumentBlock>();

        public string? Issuer { get; set; }

        public DateTime? IssueDate { get; set; }

        public string? DocumentNo { get; set; }

        public string? SignerName { get; set; }

        public bool IsLaw { get; set; } = false;

    }
}
