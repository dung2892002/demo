using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class FileRepository(ApplicationDbContext dbContext) : IFileRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(UserFile file)
        {
            await _dbContext.Files.AddAsync(file);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(UserFile file)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserFile>> GetByFoler(Guid folderId)
        {
            var files = await _dbContext.Files.Where(f => f.FolderId == folderId).ToListAsync();
            return files;
        }

        public async Task<UserFile?> GetById(Guid id)
        {
            return await _dbContext.Files.AsNoTracking().SingleOrDefaultAsync(f => f.FileId == id);
        }
    }
}
