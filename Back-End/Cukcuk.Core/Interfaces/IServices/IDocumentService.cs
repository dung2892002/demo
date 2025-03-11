using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface IDocumentService
    {
        Task<PageResult<Document>> FilterDocument(Guid? parentId, string? keyword, int pageSize, int pageNumber, Guid? categoryId, DocumentType? type);

        Task Create(Document document);

        Task Update(Guid id, Document document);

        Task Delete(Guid id);

        Task<IEnumerable<DocumentCategory>> FindAllCategory();

        Task<Document?> GetById(Guid id);

        Task CreateFile(List<IFormFile> files, Guid? parentId, Guid categoryId);

        Task<string> GetFileDetailHtml(Guid id);

        Task<IEnumerable<Document>> GetSubsDocumentFolder(Guid? parentId);

        Task MoveDocument(Guid id, Guid? parentId);
        Task<IEnumerable<Document>> GetParentDocuments(Guid id);

        Task<byte[]> CreateExcelFile(Guid? folderId, string? keyword);

        Task DeleteRange(List<Guid> ids);

        Task MoveRange(List<Guid> ids, Guid? parentId);
    }
}
