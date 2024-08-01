namespace Cukcuk.Core.DTOs
{
    public class EmployeeDTO
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string? GenderName
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
        public string? IdentityNumber { get; set; } 
        public DateTime IdentityDate { get; set; }
        public string? IdentityPlace { get; set; } 
        public string? Address { get; set; } 
        public string? MobileNumber { get; set; } 
        public string? LandlineNumber { get; set; }
        public string? Email {  get; set; }
        public string? BankNumber { get; set; } 
        public string? BankName { get; set; } 
        public string? BankBranch { get; set; }
        public Guid PositionId { get; set; }
        public string? PositionCode { get; set; }
        public string? PositionName { get; set; }
        public Guid DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
