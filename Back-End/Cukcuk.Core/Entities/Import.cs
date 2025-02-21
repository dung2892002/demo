namespace Cukcuk.Core.Entities
{
    public class Import
    {
        public int? Id { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
    }
}
