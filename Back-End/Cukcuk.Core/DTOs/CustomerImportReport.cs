namespace Cukcuk.Core.DTOs
{
    public class CustomerImportReport : CustomerExport
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
