using Cukcuk.Core.Entities;

namespace Cukcuk.Core.DTOs
{
    public class UploadFileReponse
    {
        public Guid CacheDataId { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();   
    }
}
