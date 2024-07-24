namespace BE__Back_End_.Payloads.Request
{
    public class EmployeeRequest
    {
        public Guid EmployeeId { get; set; }
        public Guid PositionId { get; set; }
        public Guid DepartmentId { get; set; }
        public string? EmployeeCode { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public DateTime IdentityDate { get; set; }
        public string IdentityPlace { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string LandlineNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
