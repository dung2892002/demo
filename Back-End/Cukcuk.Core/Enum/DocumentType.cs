using System.Text.Json.Serialization;

namespace Cukcuk.Core.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DocumentType
    {
        Folder,
        Word,
        Pdf,
        Excel,
        Ppt,
        Image,
        Unknown
    }
}
