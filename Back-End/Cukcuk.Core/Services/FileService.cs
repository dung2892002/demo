using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Services
{
    public class FileService(IFileRepository fileRepository, IFirebaseStorageService firebaseStorageService) : IFileService
    {
        private readonly IFileRepository _fileRepository = fileRepository;
        private readonly IFirebaseStorageService _firebaseStorageService = firebaseStorageService;
        public async Task<UserFile?> Create(Guid folderId, List<IFormFile> files)
        {
            var filePaths = new List<string>();
            foreach (var fileData in files)
            {
                if (fileData.Length > 0)
                {
                    var path = DateTime.Now.ToString("dd-MM-yyyy") + "_" + Guid.NewGuid().ToString() + "_" + fileData.FileName;
                    using Stream stream = fileData.OpenReadStream();
                    var result = await _firebaseStorageService.UploadFile(stream, path);

                    var file = new UserFile()
                    {
                        FileId = Guid.NewGuid(),
                        FileName = fileData.FileName,
                        FilePath = path,
                        FolderId = folderId,
                        Folder = null
                    };
                    await _fileRepository.Create(file);
                    return file;
                }
                return null;
            }
            return null;
        }

        public async Task Delete(Guid fileId)
        {
            var file = await _fileRepository.GetById(fileId) ?? throw new Exception("File not found");
            await _firebaseStorageService.Delete(file.FilePath);
            await _fileRepository.Delete(file);
        }

        public async Task<IEnumerable<UserFile>> GetByFoler(Guid folderId)
        {
            return await _fileRepository.GetByFoler(folderId);
        }

        public async Task<string> GetFileUrl(Guid fileId)
        {
            var file = await _fileRepository.GetById(fileId) ?? throw new ArgumentException("fileId not exist");

            return await _firebaseStorageService.GetFileUrl(file.FilePath);
        }
    }
}
