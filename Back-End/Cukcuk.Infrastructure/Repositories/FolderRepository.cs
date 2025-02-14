using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class FolderRepository(ApplicationDbContext dbContext) : IFolderRepository
    {

        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(Folder folder)
        {
            await _dbContext.AddAsync(folder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteFolder(Guid folderId)
        {
            var folder = await _dbContext.Folders.SingleOrDefaultAsync(f => f.Id == folderId) ?? throw new ArgumentException("folder not exist");
            _dbContext.Folders.Remove(folder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Folder>> GetByMenuId(Guid menuId)
        {
            var folders = await _dbContext.Folders.Where(f => f.MenuId == menuId).OrderBy(f => f.FolderName).ToListAsync();
            return folders;
        }

        public async Task<Folder?> GetFolderById(Guid id)
        {
            return await _dbContext.Folders.SingleOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Folder>> GetSubFolders(Guid parentId)
        {
            var folders = await _dbContext.Folders.Where(f => f.ParentId == parentId).OrderBy(f => f.FolderName).ToListAsync();
            return folders;
        }

        public async Task UpdateFolder(Folder folder)
        {
            _dbContext.Folders.Update(folder);
            await _dbContext.SaveChangesAsync();
        }
    }
}
