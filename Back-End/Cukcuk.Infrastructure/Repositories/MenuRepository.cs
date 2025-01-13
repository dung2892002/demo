using Cukcuk.Core.Auth;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class MenuRepository(ApplicationDbContext dbContext) : IMenuRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(Menu entity)
        {
            await _dbContext.Menus.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Menu menu)
        {
            _dbContext.Menus.Remove(menu);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(Guid id)
        {
            var menu = await _dbContext.Menus.SingleOrDefaultAsync(m => m.Id == id) ?? throw new ArgumentException("menu not exist");
            _dbContext.Menus.Remove(menu);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Menu?>> FindAll()
        {
            return await _dbContext.Menus.AsNoTracking().OrderBy(m => m.MenuOrder).ToListAsync();
        }

        public async Task<Menu?> FindById(Guid? id)
        {
            return await _dbContext.Menus.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task Update(Menu entity)
        {
            _dbContext.Menus.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
