using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface IMenuService : IBaseService<Menu>
    {
        Task UpdateOrder(List<Menu> menuList);
    }
}
