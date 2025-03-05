using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.RegularExpressions;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Cukcuk.Infrastructure.Repositories
{
    public class DocumentRepository(ApplicationDbContext dbContext) : IDocumentRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task Create(Document document)
        {
            await _dbContext.Documents.AddAsync(document);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Document document)
        {
            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();
        }
        private async Task<List<Guid>> GetAllChildrenIds(Guid? parentId, string keyword)
        {
            keyword = keyword.ToLower().Trim();

            var query = _dbContext.Documents.AsNoTracking().AsQueryable();

            if (parentId.HasValue)
            {
                query = query.Where(d => d.ParentId == parentId);
            }

            var children = await query
                .Where(d => d.Name.ToLower().Contains(keyword))
                .Select(d => d.Id)
                .ToListAsync();

            var allChildrenIds = new List<Guid>(children);

            foreach (var childId in children)
            {
                allChildrenIds.AddRange(await GetAllChildrenIds(childId, keyword)); 
            }

            return allChildrenIds;
        }

        public async Task<PageResult<Document>> FilterDocument(Guid? parentId, string? keyword, int pageSize, int pageNumber, Guid? categoryId, DocumentType? type)
            {
                var query = _dbContext.Documents.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                var relevantIds = await GetAllChildrenIds(parentId, keyword);

                query = query.Where(d => relevantIds.Contains(d.Id));
            }
            else
            {
                query = query.Where(d => parentId.HasValue ? d.ParentId == parentId.Value : d.ParentId == null);
            }

            if (categoryId != null)
                {
                    query = query.Where(d => d.CategoryId == categoryId);
                }

                if (type != null)
                {
                    query = query.Where(d => d.Type == type);
                }

                var totalItems = await query.CountAsync();

                var documents = await query.OrderBy(d => d.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(d => d.Category).ToListAsync();

                return new PageResult<Document>
                {
                    Items = documents,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                };
            }

        public async Task<IEnumerable<DocumentCategory>> FindAllCategory()
        {
            return await _dbContext.DocumentCategories.ToListAsync();
        }

        public async Task<Document?> GetById(Guid? id)
        {
            return await _dbContext.Documents.AsNoTracking().SingleOrDefaultAsync(d  => d.Id == id);
        }

        public async Task<IEnumerable<string>> GetFilePathToDelete(Guid id)
        {
            var paths = new List<string>();

            var documents = await _dbContext.Documents.Where(d => d.ParentId == id).ToListAsync();

            foreach (var document in documents)
            {
                if (document.Path != null) paths.Add(document.Path);

                var childPaths = await GetFilePathToDelete(document.Id);
                paths.AddRange(childPaths);
            }

            return paths;
        }

        public async Task<IEnumerable<Document>> GetSubsDocumentFolder(Guid? parentId)
        {
            var documents = await _dbContext.Documents
                .Where(d => d.ParentId == parentId)
                .Where(d => d.Type == DocumentType.Folder)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return documents;
        }

        public async Task<string> GetUniqueDocumentName(Guid? parentId, string inputName, DocumentType type, Guid? documentId)
        {
            string baseName = inputName.Trim();

            var existingFolders = await _dbContext.Documents
                                .Where(d => d.ParentId == parentId)
                                .Where(d => d.Type == type)
                                .Where(d => d.Id != documentId)
                                .Where(d => d.Name.StartsWith(baseName))
                                .Select(d => d.Name)
                                .ToListAsync();

            if (!existingFolders.Contains(baseName))
            {
                return baseName;
            }

            var existingNumbers = new HashSet<int>();
            var regex = new Regex(@$"^{Regex.Escape(baseName)}\s*\((\d+)\)$");

            foreach (var name in existingFolders)
            {
                var match = regex.Match(name);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                {
                    existingNumbers.Add(num);
                }
            }

            int newNumber = 1;
            while (existingNumbers.Contains(newNumber))
            {
                newNumber++;
            }

            return $"{baseName} ({newNumber})";
        }

        public async Task Update(Document document)
        {
            _dbContext.Documents.Update(document);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Document>> GetSubsDocument(Guid parentId)
        {
            return await _dbContext.Documents.AsNoTracking().Where(d => d.ParentId == parentId).ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetParentDocuments(Guid id)
        {
            
            var document = await _dbContext.Documents.SingleOrDefaultAsync(d => d.Id == id) ?? throw new ArgumentException("Document not exist");

            var documents = new List<Document>();

            await GetParent(document, documents);

            return documents;
        }

        private async Task GetParent(Document document, List<Document> documents)
        {

            if (document.ParentId == null)
            {
                return;
            }

            var parent = await _dbContext.Documents.SingleOrDefaultAsync(d => d.Id == document.ParentId) ?? throw new ArgumentException("Document not exist");

            documents.Add(parent);

            await GetParent(parent, documents);
        }
    }
}
