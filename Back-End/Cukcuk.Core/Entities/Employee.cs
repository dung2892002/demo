namespace Cukcuk.Core.Entities
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = new DateTime();
        public int Gender { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public DateTime IdentityDate { get; set; } = new DateTime();
        public string IdentityPlace { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string LandlineNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;
        public Department Department { get; set; } = new Department();
        public Position Position { get; set; } = new Position();
        public DateTime CreatedDate { get; set; } = new DateTime();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; } = new DateTime();
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
