namespace Cukcuk.Core.Entities
{
    public class Menu
    {
        public Guid? Id { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuPath { get; set; } = string.Empty;
        public string MenuIcon { get; set; } = string.Empty;
        public int MenuOrder { get; set; }
        public List<Folder> Folders { get; set; } = new List<Folder>();
    }
}
