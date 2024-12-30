
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using jwtAuth.Auth;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class CustomerGroupRepository(ApplicationDbContext dbContext) : ICustomerGroupRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public Task Create(CustomerGroup entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CustomerGroup?>> FindAll()
        {
            return await _dbContext.CustomerGroups.ToListAsync();
        }

        public Task<CustomerGroup?> FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerGroup?> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task Update(CustomerGroup entity)
        {
            throw new NotImplementedException();
        }
    }
}
