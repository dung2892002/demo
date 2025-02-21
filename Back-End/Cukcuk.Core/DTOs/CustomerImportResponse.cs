using Cukcuk.Core.Entities;

namespace Cukcuk.Core.DTOs
{
    public class CustomerImportResponse : Customer
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
