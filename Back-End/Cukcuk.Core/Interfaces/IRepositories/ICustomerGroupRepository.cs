using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface ICustomerGroupRepository : IBaseRepository<CustomerGroup>
    {
        Task<CustomerGroup?> GetByName(string name);
    }
}
