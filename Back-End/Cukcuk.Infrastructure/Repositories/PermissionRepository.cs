using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class PermissionRepository(ApplicationDbContext dbContext) : IPermissionRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task AddPermissionUser(UserPermission userPermission)
        {
            await _dbContext.UserPermissions.AddAsync(userPermission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckUserPermission(UserPermission userPermission)
        {
            var userPermisstion = await _dbContext.UserPermissions.AsNoTracking().SingleOrDefaultAsync(up => up.UserId == userPermission.UserId &&up.PermissionId==userPermission.PermissionId);
            return userPermisstion == null;
        }

        public async Task<bool> CheckUserPermission(string username, string permissionnName)
        {
            var userPermisstion = await _dbContext.UserPermissions.AsNoTracking().SingleOrDefaultAsync(up => up.User.UserName == username && up.Permission.PermissionName == permissionnName);
            return userPermisstion != null;
        }

        public async Task Create(Permission permission)
        {
            await _dbContext.Permissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePermissionUser(UserPermission userPermission)
        {
            _dbContext.UserPermissions.Remove(userPermission);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<IEnumerable<Permission>> FindByName(string? name)
        {
            var query = _dbContext.Permissions.AsQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(p => p.PermissionName.Contains(name));
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserPermission>> GetAllPermissionUsers(string userId)
        {
            return await _dbContext.UserPermissions.Where(p => p.UserId == userId).Include(p => p.Permission).OrderBy(p => p.CreatedDate).ToListAsync();
        }
    }
}
