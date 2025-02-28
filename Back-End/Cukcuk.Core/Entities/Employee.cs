using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public Department? Department { get; set; } = new Department();

        public Guid DepartmentId { get; set; }

        [JsonIgnore]
        public Position? Position { get; set; } = new Position();

        public Guid PositionId { get; set; }
        public DateTime CreatedDate { get; set; } = new DateTime();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; } = new DateTime();
        public string ModifiedBy { get; set; } = string.Empty;

        [JsonIgnore]
        public List<EmployeeFolder> EmployeeFolders { get; set; } = new List<EmployeeFolder>();

        [NotMapped]
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
    }
}
