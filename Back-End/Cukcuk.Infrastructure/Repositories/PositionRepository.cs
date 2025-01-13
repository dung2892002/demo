using Cukcuk.Core.Auth;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class PositionRepository(ApplicationDbContext dbContext) : IPositionRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(Position position)
        {
            await _dbContext.Positions.AddAsync(position);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(Guid id)
        {
            var position = await _dbContext.Positions.SingleOrDefaultAsync(p => p.PositionId == id) ?? throw new ArgumentException("position not exist");
            _dbContext.Positions.Remove(position);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Position?>> FindAll()
        {
            return await _dbContext.Positions.ToListAsync();
        }

        public async Task<Position?> FindById(Guid? id)
        {
            return await _dbContext.Positions.SingleOrDefaultAsync(p => p.PositionId == id);
        }

        public async Task<Position?> GetByName(string name)
        {
            return await _dbContext.Positions.SingleOrDefaultAsync(p => p.PositionName == name);
        }

        public async Task Update(Position position)
        {
            _dbContext.Positions.Update(position);
            await _dbContext.SaveChangesAsync();
        }
    }
}
