﻿using Cukcuk.Core.DTOs;
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
        Task<IEnumerable<Document>> GetSubsDocument(Guid parentId);

        Task<IEnumerable<Document>> GetParentDocuments(Guid id);

        Task<IEnumerable<Document>> GetListDocument(Guid? folderId, string? keyword);
        Task<IEnumerable<Document>> GetListDocumentsById(List<Guid> ids);
        Task DeleteRange(IEnumerable<Document> document);

        Task AddBlockRange(List<DocumentBlock> blocks);

        Task<List<DocumentBlock>> GetBlockByDocumentId(Guid documentId);

        Task<DocumentBlock?> GetBlockById(Guid blockId);

        Task UpdateBlock(DocumentBlock block);

        Task UpdateBlocks(List<DocumentBlock> updateBlocks, List<DocumentBlock> addBlocks, List<DocumentBlock> deleteBlocks);   
    }
}
