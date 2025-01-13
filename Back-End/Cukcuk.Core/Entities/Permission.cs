using System.Text.Json.Serialization;

namespace Cukcuk.Core.Entities
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
