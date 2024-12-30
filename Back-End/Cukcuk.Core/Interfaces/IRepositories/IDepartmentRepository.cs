using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department?> GetByName(string name);   
    }
}
