using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.IServices;

namespace Cukcuk.Core.Services
{
    public class MenuService(IMenuRepository menuRepository) : IMenuService
    {
        private readonly IMenuRepository _menuRepository = menuRepository;
        public async Task Create(Menu entity)
        {
            entity.Id = Guid.NewGuid();
            await _menuRepository.Create(entity);
        }

        public async Task DeleteById(Guid id)
        {
            await _menuRepository.DeleteById(id);
        }

        public async Task<IEnumerable<Menu?>> GetAll()
        {
            return await _menuRepository.FindAll();
        }

        public async Task<Menu?> GetById(Guid id)
        {
            return await _menuRepository.FindById(id);
        }

        public async Task Update(Guid id, Menu entity)
        {
            var import = await _menuRepository.FindById(id) ?? throw new ArgumentException("import not exist"); 
            entity.Id = import.Id;
            await _menuRepository.Update(entity);
        }

        public async Task UpdateOrder(List<Menu> menuList)
        {
            var allMenus = await _menuRepository.FindAll();
            if (menuList.Count != allMenus.Count())
            {
                var menuDeletes = allMenus.Where(m => !menuList.Any(menu => menu.Id == m.Id));
                foreach (var menu in menuDeletes)
                {
                    await _menuRepository.Delete(menu);
                }
            }
            foreach (var menu in menuList)
            {
                var menuExistng = await _menuRepository.FindById(menu.Id) ?? throw new ArgumentException("import not exist");
                if (menuExistng.MenuOrder != menu.MenuOrder)
                {
                    menuExistng.MenuOrder = menu.MenuOrder;
                    await _menuRepository.Update(menu);
                }
            }
        }
    }
}
