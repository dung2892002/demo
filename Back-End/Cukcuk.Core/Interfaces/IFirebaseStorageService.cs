using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFile(Stream fileStream, string path);

        Task<string> GetFileUrl(string path);
        Task Delete(string path);
        Task<IFormFile> DownloadFileAsIFormFile(string path);
    }
}
