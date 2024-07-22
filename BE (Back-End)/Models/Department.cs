namespace BE__Back_End_.Models
{
    public class Department
    {
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
