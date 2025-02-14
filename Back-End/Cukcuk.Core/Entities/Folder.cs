using System.Text.Json.Serialization;


namespace Cukcuk.Core.Entities
{
    public class Folder
    {
        public Guid? Id { get; set; }
        public string FolderName { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;

        public Guid? MenuId { get; set; }

        [JsonIgnore]
        public Menu? Menu { get; set; }

        public Guid? ParentId { get; set; }

        [JsonIgnore]
        public Folder? Parent { get; set; }

        [JsonIgnore]
        public List<Folder> SubFolders { get; set;} = new List<Folder>();

        [JsonIgnore]
        public List<UserFile> Files { get; set; } = new List<UserFile> { };
    }
}
