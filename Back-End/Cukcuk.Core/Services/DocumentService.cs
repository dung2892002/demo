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
using Cukcuk.Core.Helper;
using Org.BouncyCastle.Crypto;

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

        public async Task CreateFile(List<IFormFile> files, Guid? parentId, Guid categoryId)
        {
            foreach (var file in files)
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
        }

        public async Task Delete(Guid id)
        {
            var exsitingdocument = await _documentRepository.GetById(id) ?? throw new ArgumentException("document not exist");

            if (exsitingdocument.Path == null)
            {
                var paths = await _documentRepository.GetFilePathToDelete(id);

                foreach (var path in paths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(exsitingdocument.Path))
                {
                    System.IO.File.Delete(exsitingdocument.Path);
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
                return ConvertWordToMarkdown(document.Path);
            if (document.Type == DocumentType.Pdf)
                return ConvertPdfToHtml(document.Path);
            return "Chi co the mo file word va pdf";
        }


        private static string ConvertWordToMarkdown(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("File không tồn tại.");
            }

            //Open a file as a stream.
            using (FileStream fileStreamPath = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //Load an existing Word document.
                using (WordDocument document = new WordDocument(fileStreamPath, FormatType.Docx))
                {
                    //Create a file stream.
                    using (FileStream outputFileStream = new FileStream("WordToMarkdown.md", FileMode.Create, FileAccess.ReadWrite))
                    {
                        //Save a Markdown file to the file stream.
                        document.Save(outputFileStream, FormatType.Markdown);
                        outputFileStream.Position = 0;

                        using (StreamReader reader = new StreamReader(outputFileStream))
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
            await HandleMoveDocument(document, parentId);
            await HandleUpdatePathSubDocument(document);

        }

        

        public async Task<IEnumerable<Document>> GetParentDocuments(Guid id)
        {
            return await _documentRepository.GetParentDocuments(id);
        }

        public async Task<byte[]> CreateExcelFile(Guid? folderId, string? keyword)
        {
            var documents = await _documentRepository.GetListDocument(folderId, keyword);
            var categories = await _documentRepository.FindAllCategory();

            var documentsExport = new List<DocumentExport>();
            var index = 1;
            foreach (var document in documents)
            {
                documentsExport.Add(new DocumentExport
                {
                    STT = index++,
                    CreatedAt = document.CreatedAt.ToString(),
                    DocumentName = document.Name,
                    Category = categories.First(c => c.Id == document.CategoryId).Name,
                });
            }

            var file = ExcelHelper.CreateFile(documentsExport);
            return file;
        }

        public async Task DeleteRange(List<Guid> ids)
        {
            var docsToDelete = await _documentRepository.GetListDocumentsById(ids);

            await _documentRepository.DeleteRange(docsToDelete);

            foreach (var document in docsToDelete)
            {
                if (document.Type == DocumentType.Folder || !System.IO.File.Exists(document.Path))
                {
                    continue;
                }
                System.IO.File.Delete(document.Path);
            }
        }

        public async Task MoveRange(List<Guid> ids, Guid? parentId)
        {
            var documents = new List<Document>();
            foreach (var id in ids)
            {
                var document = await _documentRepository.GetById(id) ?? throw new ArgumentException("not exist");

                await HandleMoveDocument(document, parentId);

                documents.Add(document);
            }

            foreach (var document in documents) 
                await HandleUpdatePathSubDocument(document);
        }

        private async Task HandleMoveDocument(Document document, Guid? parentId)
        {

            document.ParentId = parentId;
            document.FolderPath = "Tài liệu";

            document.Name = await _documentRepository.GetUniqueDocumentName(parentId, document.Name, document.Type, null);

            if (parentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }
            await _documentRepository.Update(document);
        }

        private async Task HandleUpdatePathSubDocument(Document parent)
        {
            var childrens = await _documentRepository.GetSubsDocument(parent.Id);
            if (!childrens.Any())
            {
                return;
            }
            foreach (var children in childrens)
            {
                children.FolderPath = parent.FolderPath + "/" + parent.Name;
                await _documentRepository.Update(children);
                await HandleUpdatePathSubDocument(children);
            }
        }
    }
}
