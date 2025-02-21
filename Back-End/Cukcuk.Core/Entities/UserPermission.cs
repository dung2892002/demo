using Cukcuk.Core.Auth;

namespace Cukcuk.Core.Entities
{
    public class UserPermission
    {
        public string UserId { get; set; } = string.Empty;
        public Guid PermissionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public ApplicationUser? User { get; set; }
        public Permission? Permission { get; set; }
    }
}
