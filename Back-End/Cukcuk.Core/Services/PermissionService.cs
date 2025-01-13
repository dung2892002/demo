using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.IServices;

namespace Cukcuk.Core.Services
{
    public class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        public async Task AddPermissionUser(UserPermission userPermission)
        {
            if (await CheckUserPermission(userPermission))
            {
                userPermission.CreatedDate = DateTime.Now;
                await _permissionRepository.AddPermissionUser(userPermission);
            }
        }

        public Task<bool> CheckUserPermission(UserPermission userPermission)
        {
            return _permissionRepository.CheckUserPermission(userPermission);
        }

        public Task<bool> CheckUserPermission(string username, string permissionnName)
        {
            return _permissionRepository.CheckUserPermission(username, permissionnName);
        }

        public async Task Create(Permission permission)
        {
            permission.PermissionId = Guid.NewGuid();
            await _permissionRepository.Create(permission);
        }

        public async Task DeletePermissionUser(UserPermission userPermission)
        {
            if (!await CheckUserPermission(userPermission)) 
                await _permissionRepository.DeletePermissionUser(userPermission);
        }

        public async Task<IEnumerable<Permission>> FindByName(string? name)
        {
            return await _permissionRepository.FindByName(name);
        }
        public async Task<IEnumerable<UserPermission>> GetAllPermissionUsers(string userId)
        {
            return await _permissionRepository.GetAllPermissionUsers(userId);
        }
    }
}
