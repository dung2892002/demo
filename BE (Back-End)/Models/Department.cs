namespace BE__Back_End_.Models
{
    public class Department
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = new DateTime();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; } = new DateTime();
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
