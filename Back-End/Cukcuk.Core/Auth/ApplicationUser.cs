using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Cukcuk.Core.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        [JsonIgnore]
        public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
