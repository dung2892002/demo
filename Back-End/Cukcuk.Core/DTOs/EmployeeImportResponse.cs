namespace Cukcuk.Core.DTOs
{
    public class EmployeeImportResponse : EmployeeDTO
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
