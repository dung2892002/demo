namespace Cukcuk.Core.DTOs
{
    public class EmployeeImportReport : EmployeeExport
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
