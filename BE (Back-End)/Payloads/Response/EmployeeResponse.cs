namespace BE__Back_End_.Payloads.DTOs
{
    public class EmployeeResponse
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string GenderName
        {
            get
            {
                if (Gender == 0)
                    return "Nữ";
                if (Gender == 1)
                    return "Nam";
                return "Không rõ";
            }
        }
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
        public Guid PositionId { get; set; }
        public string PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
