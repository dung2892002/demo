using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IFolderRepository
    {
        Task Create(Folder folder);
        Task<IEnumerable<Folder>> GetByMenuId(Guid menuId);
        Task<IEnumerable<Folder>> GetSubFolders(Guid parentId);
        Task UpdateFolder(Folder folder);
        Task DeleteFolder(Guid folderId);

        Task<Folder?> GetFolderById(Guid id);
    }
}
