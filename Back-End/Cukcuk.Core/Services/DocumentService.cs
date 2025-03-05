using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;

namespace Cukcuk.Core.Services
{
    public class DocumentService(IDocumentRepository documentRepository) : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        public async Task Create(Document document)
        {
            document.Parent = null;
            document.Category = null;
            document.Children = new List<Document>();

            document.Name = await _documentRepository.GetUniqueDocumentName(document.ParentId, document.Name, DocumentType.Folder, null);
            document.Path = null;
            document.Id = Guid.NewGuid();
            document.CreatedAt = DateTime.Now;
            document.Type = DocumentType.Folder;
            document.FolderPath = "Tài liệu";

            if (document.ParentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }

            await _documentRepository.Create(document);
        }

        public async Task CreateFile(IFormFile file, Guid? parentId, Guid categoryId)
        {
            ArgumentNullException.ThrowIfNull(file);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine("wwwroot/files", uniqueFileName).Replace("\\", "/"); ;

            var document = new Document
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                CategoryId = categoryId,
                CreatedAt = DateTime.Now,
                Parent = null,
                Category = null,
                Children = new List<Document>(),
                Path = filePath,
                FolderPath = "Tài liệu",
                Name = Path.GetFileNameWithoutExtension(file.FileName),
                Type = GetDocumentType(file)
            };

            document.Name = await _documentRepository.GetUniqueDocumentName(parentId, document.Name, document.Type, null);

            if (document.ParentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await _documentRepository.Create(document);
        }

        public async Task Delete(Guid id)
        {
            var exsitingdocument = await _documentRepository.GetById(id) ?? throw new ArgumentException("document not exist");

            if (exsitingdocument.Path != null)
            {
                if (System.IO.File.Exists(exsitingdocument.Path))
                {
                    System.IO.File.Delete(exsitingdocument.Path);
                }
            }
            else
            {
                var paths = await _documentRepository.GetFilePathToDelete(id);

                foreach (var path in paths)
                {
                    Console.WriteLine(path);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }

            await _documentRepository.Delete(exsitingdocument);
        }

        public async Task<PageResult<Document>> FilterDocument(Guid? parentId, string? keyword, int pageSize, int pageNumber, Guid? categoryId, DocumentType? type)
        {
            return await _documentRepository.FilterDocument(parentId, keyword, pageSize, pageNumber, categoryId, type);
        }

        public async Task<IEnumerable<DocumentCategory>> FindAllCategory()
        {
            return await _documentRepository.FindAllCategory();
        }

        public Task<Document?> GetById(Guid id)
        {
            return _documentRepository.GetById(id);
        }

        public async Task Update(Guid id,Document document)
        {
           var exsitingdocument = await _documentRepository.GetById(id) ?? throw new ArgumentException("document not exist");

            exsitingdocument.Parent = null;
            exsitingdocument.Category = null;
            exsitingdocument.Children = new List<Document>();

            exsitingdocument.CategoryId = document.CategoryId;
            exsitingdocument.Name = await _documentRepository.GetUniqueDocumentName(document.ParentId, document.Name, document.Type, exsitingdocument.Id);

            await _documentRepository.Update(exsitingdocument);
            await HandleUpdatePathSubDocument(document);
        }


        private static DocumentType GetDocumentType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            return extension switch
            {
                ".doc" or ".docx" => DocumentType.Word,
                ".xls" or ".xlsx" => DocumentType.Excel,
                ".pdf" => DocumentType.Pdf,
                ".ppt" or ".pptx" => DocumentType.Ppt,
                ".jpg" or ".jpeg" or ".png" or ".gif" => DocumentType.Image,
                _ => DocumentType.Unknown,
            };
        }

        public async Task<string> GetFileDetailHtml(Guid id)
        {
            var document = await _documentRepository.GetById(id)
                ?? throw new ArgumentException($"ID: {id} không tồn tại trong hệ thống");

            if (!File.Exists(document.Path))
                throw new FileNotFoundException($"File {document.Path} không tồn tại.");

            if (document.Type == DocumentType.Word)
                return ConvertWordToHtml(document.Path);
            if (document.Type == DocumentType.Pdf)
                return ConvertPdfToHtml(document.Path);
            return "Chi co the mo file word va pdf";
        }

        private static string ConvertWordToHtml(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("file not found");
            }

            using (FileStream docStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (WordDocument document = new WordDocument(docStream, FormatType.Docx))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        document.Save(outputStream, FormatType.Html);

                        outputStream.Position = 0;

                        using (StreamReader reader = new StreamReader(outputStream))
                        {
                            string markdownContent = reader.ReadToEnd();

                            return markdownContent;
                        }
                    }
                }
            }
        }

        private static string ConvertPdfToHtml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File không tồn tại.");
            }

            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringWriter htmlContent = new StringWriter();

                htmlContent.WriteLine("<html><body>");

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);

                    htmlContent.WriteLine($"<p>{text}</p>");
                }

                htmlContent.WriteLine("</body></html>");
                return htmlContent.ToString();
            }
        }

        public async Task<IEnumerable<Document>> GetSubsDocumentFolder(Guid? parentId)
        {
            return await _documentRepository.GetSubsDocumentFolder(parentId);
        }

        public async Task MoveDocument(Guid id, Guid? parentId)
        {
            var document = await _documentRepository.GetById(id) ?? throw new ArgumentException("document not exist");

            document.ParentId = parentId;
            document.FolderPath = "Tài liệu";

            document.Name = await _documentRepository.GetUniqueDocumentName(parentId, document.Name, document.Type, null);

            if (parentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }
            await _documentRepository.Update(document);
            await HandleUpdatePathSubDocument(document);

        }

        private async Task HandleUpdatePathSubDocument(Document parent) 
        {
            var childrens = await _documentRepository.GetSubsDocument(parent.Id);
            if (!childrens.Any()) return;
            foreach (var children in childrens)
            {
                children.FolderPath = parent.FolderPath + "/" + parent.Name;
                await _documentRepository.Update(children);
                await HandleUpdatePathSubDocument(children);
            }
        }

        public async Task<IEnumerable<Document>> GetParentDocuments(Guid id)
        {
            return await _documentRepository.GetParentDocuments(id);
        }
    }
}
