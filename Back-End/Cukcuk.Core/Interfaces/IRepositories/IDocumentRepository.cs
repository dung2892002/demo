using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IDocumentRepository
    {
        Task<PageResult<Document>> FilterDocument(Guid? parentId, string? keyword, int pageSize, int pageNumber, Guid? categoryId, DocumentType? type);

        Task Create(Document document);

        Task Update(Document document);

        Task Delete(Document document);

        Task<IEnumerable<DocumentCategory>> FindAllCategory();

        Task<Document?> GetById(Guid? id);

        Task<IEnumerable<string>> GetFilePathToDelete(Guid id);

        Task<IEnumerable<Document>> GetSubsDocumentFolder(Guid? parentId);

        Task<string> GetUniqueDocumentName(Guid? parentId, string inputName, DocumentType type, Guid? documentId);
    }
}
