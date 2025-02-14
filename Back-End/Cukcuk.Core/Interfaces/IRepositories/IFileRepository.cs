using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<UserFile>> GetByFoler(Guid folderId);

        Task Create(UserFile file);

        Task Delete(UserFile file);

        Task<UserFile?> GetById(Guid id);
    }
}
