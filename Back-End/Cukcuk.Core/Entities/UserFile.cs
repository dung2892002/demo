using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class UserFile
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        public Guid FolderId { get; set; }

        [JsonIgnore]
        public Folder? Folder { get; set; }
    }
}
