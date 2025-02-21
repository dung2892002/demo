using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> FindByName(string? name);
        Task Create(Permission permission);
        Task<bool> CheckUserPermission(UserPermission userPermission);
        Task<bool> CheckUserPermission(string username, string permissionnName);
        Task AddPermissionUser(UserPermission userPermission);
        Task DeletePermissionUser(UserPermission userPermission);
        Task<IEnumerable<UserPermission>> GetAllPermissionUsers(string userId);
    }
}
