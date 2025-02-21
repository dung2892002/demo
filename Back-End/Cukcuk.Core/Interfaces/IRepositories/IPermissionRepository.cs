using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> FindByName(string? name);
        Task Create(Permission permission);
        Task<bool> CheckUserPermission(UserPermission userPermission);
        Task AddPermissionUser(UserPermission userPermission);
        Task DeletePermissionUser(UserPermission userPermission);
        Task<bool> CheckUserPermission(string username, string permissionnName);
        Task<IEnumerable<UserPermission>> GetAllPermissionUsers(string userId);
    }
}
