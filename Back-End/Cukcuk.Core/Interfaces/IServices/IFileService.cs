using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface IFileService
    {
        Task<IEnumerable<UserFile>> GetByFoler(Guid folderId);

        Task<UserFile?> Create(Guid folderId, List<IFormFile> files);
        Task<string> GetFileUrl(Guid fileId);

        Task Delete(Guid fileId);
    }
}
