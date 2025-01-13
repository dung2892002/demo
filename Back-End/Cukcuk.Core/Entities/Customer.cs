namespace Cukcuk.Core.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; } = new DateTime();
        public int Gender { get; set; }
        private string? _genderName;
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
            set
            {
                _genderName = value;
                if (_genderName == "Nữ")
                    Gender = 0;
                else if (_genderName == "Nam")
                    Gender = 1;
                else
                    Gender = 2;
            }
        }
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public CustomerGroup Group { get; set; } = new CustomerGroup();
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }
}
