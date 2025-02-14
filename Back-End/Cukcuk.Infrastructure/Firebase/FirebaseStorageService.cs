using Cukcuk.Core.Interfaces;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Cukcuk.Infrastructure.Firebase
{
    public class FirebaseStorageService(IConfiguration configuration) : IFirebaseStorageService
    {
        private readonly IConfiguration _configuration = configuration;
        public async Task Delete(string path)
        {
            try
            {
                var bucket = _configuration["Firebase:Bucket"];
                var task = new FirebaseStorage(bucket).Child("files").Child(path).DeleteAsync();
                await task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file {path}: {ex.Message}");
            }
        }

        public async Task<string> GetFileUrl(string path)
        {
            try
            {
                var bucket = _configuration["Firebase:Bucket"];

                var task = new FirebaseStorage(bucket).Child("files").Child(path).GetDownloadUrlAsync();
                string downloadUrl = await task;
                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("File not found");
            }
        }

        public async Task<string> UploadFile(Stream fileStream, string path)
        {
            try
            {
                var bucket = _configuration["Firebase:Bucket"];
                var task = new FirebaseStorage(bucket).Child("files").Child(path).PutAsync(fileStream);
                string downloadUrl = await task;
                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<IFormFile> DownloadFileAsIFormFile(string path)
        {
            try
            {
                var bucket = _configuration["Firebase:Bucket"];
                var task = new FirebaseStorage(bucket).Child("files").Child(path).GetDownloadUrlAsync();
                string downloadUrl = await task;

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("File not found or cannot be downloaded.");
                }

                // Đọc nội dung của file thành một Stream
                var fileStream = await response.Content.ReadAsStreamAsync();

                // Lấy thông tin content-type
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                // Chuyển đổi thành IFormFile
                var fileName = Path.GetFileName(path);
                var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };

                return formFile;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file: {ex.Message}");
            }
        }
    }
}
