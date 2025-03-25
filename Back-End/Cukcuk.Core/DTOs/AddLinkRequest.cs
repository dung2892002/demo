namespace Cukcuk.Core.DTOs
{
    public class AddLinkRequest
    {
        public string Link { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid? ParentId { get; set; }
    }
}
