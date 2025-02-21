using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task Delete(Menu menu);
    }
}
