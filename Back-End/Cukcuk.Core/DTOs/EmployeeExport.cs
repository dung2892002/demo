namespace Cukcuk.Core.DTOs
{
    public class EmployeeExport
    {
        public int STT {  get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public DateTime? IdentityDate { get; set; }
        public string IdentityPlace { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string LandlineNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
