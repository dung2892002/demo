using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class DepartmentRepository(ApplicationDbContext dbContext) : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task Create(Department department)
        {
            await _dbContext.Departments.AddAsync(department);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(Guid id)
        {
            var department = await _dbContext.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id) ?? throw new ArgumentException("department not exist");
            _dbContext.Departments.Remove(department);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Department?>> FindAll()
        {
            return await _dbContext.Departments.ToListAsync();
        }

        public async Task<Department?> FindById(Guid? id)
        {
           return await _dbContext.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<Department?> GetByName(string name)
        {
            return await _dbContext.Departments.SingleOrDefaultAsync(d => d.DepartmentName == name);
        }

        public async Task Update(Department department)
        {
            _dbContext.Departments.Update(department);
            await _dbContext.SaveChangesAsync();
        }
    }
}
