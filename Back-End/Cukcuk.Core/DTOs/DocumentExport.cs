namespace Cukcuk.Core.DTOs
{
    public class DocumentExport
    {
        public int STT { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CustomerGroup { get; set; } = string.Empty;
    }
}
