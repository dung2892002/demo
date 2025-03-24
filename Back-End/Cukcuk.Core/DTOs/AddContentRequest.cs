namespace Cukcuk.Core.DTOs
{
    public class AddContentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid? ParentId { get; set; }
    }
}
