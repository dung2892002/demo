
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class DocumentBlock
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid DocumentId { get; set; }
        public int Level { get; set; }
        public int ContentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }

        [JsonIgnore]
        public Document? Document { get; set; }

        [JsonIgnore]
        public DocumentBlock? Parent { get; set; }

        [JsonIgnore]
        public List<DocumentBlock> Childrens { get; set; } = new List<DocumentBlock>();

        [NotMapped]
        public int? State { get; set; }

    }
}
