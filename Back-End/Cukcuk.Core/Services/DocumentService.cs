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
using System.Text.RegularExpressions;
using System.Globalization;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Cukcuk.Core.Services
{
    public class DocumentService(IDocumentRepository documentRepository, Cache cache, IImportRepository importRepository) : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        private readonly Cache _cache = cache;
        private readonly IImportRepository _importRepository = importRepository;


        private static (string, string) ReadWebFormLink(string link)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");

                using var driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(link);
                var title = driver.Title;
                var content = driver.FindElement(By.TagName("body")).Text;
                driver.Quit();
                return (title, content);
            }
            catch (Exception)
            {
                throw new InvalidDataException("Link bị lỗi, kiểm tra lại");
            }
        }

        public async Task<Guid> CreateLink(AddLinkRequest request)
        {
            var (title, content) = ReadWebFormLink(request.Link);

            var document = new Document();
            document.Id = Guid.NewGuid();
            document.CreatedAt = DateTime.Now;
            document.Type = DocumentType.Link;
            document.Name = title;
            document.ParentId = request.ParentId;
            document.CategoryId = request.CategoryId;
            document.FolderPath = "Tài liệu";

            document.Name = await _documentRepository.GetUniqueDocumentName(request.ParentId, document.Name, document.Type, null);
            if (document.ParentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }

            var block = new DocumentBlock();
            block.Id = Guid.NewGuid();
            block.DocumentId = document.Id;
            block.Content = content;
            block.Title = request.Link;

            var blocks = new List<DocumentBlock>();
            blocks.Add(block);

            await _documentRepository.Create(document);
            await _documentRepository.AddBlockRange(blocks);

            return document.Id;
        }

        public async Task UpdateContentBlock(Guid id, string newContent)
        {
            var block = await _documentRepository.GetBlockById(id) ?? throw new ArgumentException("Block not exist");

            block.Content = newContent;

            await _documentRepository.UpdateBlock(block);
        }

        public async Task<Guid> CreateContentFile(IFormFile file, Guid? parentId, Guid categoryId)
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                CategoryId = categoryId,
                CreatedAt = DateTime.Now,
                Parent = null,
                Category = null,
                Children = new List<Document>(),
                Path = "",
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


            var imports = await _importRepository.GetByTable("DocumentBlock");
            var blocks = await ExcelHelper.ReadFile<DocumentBlock>(file, imports);

            for (int i = 0; i < blocks.Count; i++) {
                blocks[i].DocumentId = document.Id;
                blocks[i].Order = i + 1;
            }
            await _documentRepository.Create(document);
            await _documentRepository.AddBlockRange(blocks);

            return document.Id;
        }

        public async Task CreateContent(AddContentRequest request)
        {
            var document = new Document();
            document.Id = Guid.NewGuid();
            document.CreatedAt = DateTime.Now;
            document.Type = DocumentType.Unknown;
            document.Name = request.Title;
            document.ParentId = request.ParentId;
            document.CategoryId = request.CategoryId;
            document.Name = await _documentRepository.GetUniqueDocumentName(request.ParentId, document.Name, document.Type, null);
            if (document.ParentId != null)
            {
                var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
            }

            var block = new DocumentBlock();
            block.Id = Guid.NewGuid();
            block.DocumentId = document.Id;
            block.Content = request.Content;
            block.Title = request.Title;

            var blocks = new List<DocumentBlock>();
            blocks.Add(block);

            await _documentRepository.Create(document);
            await _documentRepository.AddBlockRange(blocks);
        }
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


            if (exsitingdocument.IsLaw)
            {
                exsitingdocument.IssueDate = document.IssueDate;
                exsitingdocument.DocumentNo = document.DocumentNo;
                exsitingdocument.Issuer = document.Issuer;
                exsitingdocument.SignerName = document.SignerName;
            }

            await _documentRepository.Update(exsitingdocument);
            await HandleUpdatePathSubDocument(document);
            await HandleUpdateBlocks(document.DocumentBlocks);
        }

        private async Task HandleUpdateBlocks(List<DocumentBlock> blocks)
        {
            Console.WriteLine(blocks.Count);
            var updateBlocks = blocks.Where(b => b.State == 1).ToList();
            var addBlocks = blocks.Where(b => b.State == 2).ToList();
            var deleteBlocks = blocks.Where(b => b.State == 3).ToList();

            await _documentRepository.UpdateBlocks(updateBlocks, addBlocks, deleteBlocks);
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

        public async Task<string> GetMarkdownReview(string path)
        {
            var pathFile = Path.Combine("wwwroot/tmp", path).Replace("\\", "/");


            if (!File.Exists(pathFile))
                throw new FileNotFoundException($"File {path} không tồn tại.");
            await Task.CompletedTask;

            return ConvertWordToMarkdown(pathFile);
        }

        public async Task<string> GetFileDetailMarkdown(Guid id)
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

                            return RemoveSyncfusionTrialMessage(markdownContent);
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

        public async Task SaveFileUpload(Guid cacheId)
        {
            var documents = _cache.GetFromCache<Document>(cacheId) ?? throw new ArgumentException("Cache khong ton tai");

            foreach (var document in documents)
            {
                document.Name = await _documentRepository.GetUniqueDocumentName(document.ParentId, document.Name, document.Type, null);

                if (document.ParentId != null)
                {
                    var parentFolder = await _documentRepository.GetById(document.ParentId) ?? throw new ArgumentException("parent folder not exist");

                    document.FolderPath = parentFolder.FolderPath + "/" + parentFolder.Name;
                }

                var blocks = document.DocumentBlocks;
                document.DocumentBlocks = new List<DocumentBlock>();

                var tempFilePath = Path.Combine("wwwroot/tmp", document.Path).Replace("\\", "/");
                var finalPath = Path.Combine("wwwroot/files", document.Path).Replace("\\", "/");
            
                System.IO.File.Move(tempFilePath, finalPath);
                document.Path = finalPath;

                await _documentRepository.Create(document);
                await _documentRepository.AddBlockRange(blocks);
            }
        }

        public async Task CancelUpload(Guid cacheId)
        {
            var documents = _cache.GetFromCache<Document>(cacheId) ?? throw new ArgumentNullException();

            foreach (var document in documents)
            {
                var tempFilePath = Path.Combine("wwwroot/tmp", document.Path).Replace("\\", "/");

                System.IO.File.Delete(tempFilePath);
            }

            _cache.RemoveCache(cacheId);
            await Task.CompletedTask;
        }

        public async Task<UploadFileReponse> UploadFile(List<IFormFile> files, Guid categoryId, Guid? parentId)
        {

            var documents = new List<Document>();

            foreach (var file in files)
            {
                ArgumentNullException.ThrowIfNull(file);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine("wwwroot/tmp", uniqueFileName).Replace("\\", "/"); 

                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    ParentId = parentId,
                    CategoryId = categoryId,
                    CreatedAt = DateTime.Now,
                    Parent = null,
                    Category = null,
                    Children = new List<Document>(),
                    Path = uniqueFileName,
                    FolderPath = "Tài liệu",
                    IsLaw = true,
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    Type = GetDocumentType(file)
                };


                

                try
                {
                    var markdownData = RemoveSyncfusionTrialMessage(ConvertWordToMarkdown(file));

                    var blocks = SplitWordDocument(markdownData, document);

                    document.DocumentBlocks = blocks;

                    documents.Add(document);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException("Chi co the tai file chuan dinh dang quy pham phap luat");
                }
            }

            var cacheDataId = Guid.NewGuid();
            _cache.SaveDataToCache<Document>(cacheDataId, documents);
            return new UploadFileReponse
            {
                CacheDataId = cacheDataId,
                Documents = documents
            };
        }



        private static string ConvertWordToMarkdown(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "Vui lòng chọn một file hợp lệ.";
            }

            using (var fileStream = file.OpenReadStream())
            {
                if (fileStream == null)
                {
                    throw new ArgumentNullException(nameof(fileStream), "File stream không được null.");
                }

                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (WordDocument document = new WordDocument(fileStream, FormatType.Docx))
                    {
                        // Duyệt qua tất cả các đoạn văn và loại bỏ hình ảnh
                        foreach (IWSection section in document.Sections)
                        {
                            foreach (IWParagraph paragraph in section.Paragraphs)
                            {
                                for (int i = paragraph.ChildEntities.Count - 1; i >= 0; i--)
                                {
                                    if (paragraph.ChildEntities[i] is WPicture)
                                    {
                                        paragraph.ChildEntities.RemoveAt(i);
                                    }
                                }
                            }
                        }

                        document.Save(outputStream, FormatType.Markdown);
                    }

                    outputStream.Position = 0;
                    using (StreamReader reader = new StreamReader(outputStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }


        private static string NormalizeMarkdownOutput(string input)
        {
            // Bước 1: Chuẩn hóa dòng trống (loại bỏ nhiều dòng trống liên tiếp)
            string normalized = Regex.Replace(input, @"(\r?\n\s*){2,}", "\n\n").Trim();

            // Bước 2: Chia thành danh sách các dòng và loại bỏ dòng trống hoặc chứa toàn khoảng trắng
            var lines = normalized
                .Split(new[] { "\n\n" }, StringSplitOptions.None) // Giữ nguyên dòng trống cần thiết
                .Select(line => line.Trim()) // Xóa khoảng trắng đầu/cuối dòng
                .Where(line => !string.IsNullOrEmpty(line)) // Bỏ dòng rỗng
                .ToList();

            // Bước 3: Gộp lại thành chuỗi, giữ nguyên định dạng markdown
            return string.Join("\n\n", lines);
        }

        private static string ExtractSignerName(string input)
        {
            // Regex để tìm đoạn **Tên người ký**
            string pattern = @"\*\*(.*?)\*\*\s*\|";

            // Thực hiện tìm kiếm
            MatchCollection matches = Regex.Matches(input, pattern);

            // Lấy tên cuối cùng (vì chức danh cũng có ** **)
            if (matches.Count > 0)
            {
                return matches[matches.Count - 1].Groups[1].Value.Trim();
            }

            return string.Empty;
        }


        private static (string CoQuan, string SoLuat, string NgayBanHanh) ExtractLawInfo(string input)
        {
            string coQuanPattern = @"\|\*\*(.*?)\*\*";
            string soLuatPattern = @"ố:\s*([\w/\-]+)";
            string ngayPattern = @"(\d{1,2}) tháng (\d{1,2}) năm (\d{4})"; 

            string coQuan = Regex.Match(input, coQuanPattern).Groups[1].Value.Trim();
            string soLuat = Regex.Match(input, soLuatPattern).Groups[1].Value.Trim();

            var ngayMatch = Regex.Match(input, ngayPattern);
            string ngayBanHanh = "";

            if (ngayMatch.Success)
            {
                string ngay = ngayMatch.Groups[1].Value;
                string thang = ngayMatch.Groups[2].Value;
                string nam = ngayMatch.Groups[3].Value;
                ngayBanHanh = $"{ngay.PadLeft(2, '0')}/{thang.PadLeft(2, '0')}/{nam}";
            }

            return (coQuan, soLuat, ngayBanHanh);
        }


        private static string RemoveSyncfusionTrialMessage(string markdownContent)
        {
            if (string.IsNullOrWhiteSpace(markdownContent))
                return markdownContent;

            string trialMessageStart = "**Created with a trial version of Syncfusion Word library or registered the wrong key in your application.";
            string trialMessageEnd = "to obtain the valid key.**";

            // Xóa phần đầu
            int startIndex = markdownContent.IndexOf(trialMessageStart);
            if (startIndex != -1)
            {
                int endIndex = markdownContent.IndexOf(trialMessageEnd, startIndex);
                if (endIndex != -1)
                {
                    markdownContent = markdownContent.Remove(startIndex, (endIndex + trialMessageEnd.Length) - startIndex);
                }
            }

            // Xóa phần cuối (nếu có)
            startIndex = markdownContent.LastIndexOf(trialMessageStart);
            if (startIndex != -1)
            {
                int endIndex = markdownContent.IndexOf(trialMessageEnd, startIndex);
                if (endIndex != -1)
                {
                    markdownContent = markdownContent.Remove(startIndex, (endIndex + trialMessageEnd.Length) - startIndex);
                }
            }

            return NormalizeMarkdownOutput(markdownContent.Trim()); // Loại bỏ khoảng trắng thừa
        }


        private static List<DocumentBlock> SplitWordDocument(string markdownData, Document document)
        {
            Stack<DocumentBlock> blocks = new Stack<DocumentBlock>();
            List<DocumentBlock> returnBlocks = new List<DocumentBlock>();

            List<string> contents = markdownData
                                    .Split(new[] { "\n\n" }, StringSplitOptions.None) // Tách từng dòng nhưng không loại bỏ gì
                                    .Select(line => line.Trim()) // Xóa khoảng trắng đầu/cuối
                                    .Where(line => !string.IsNullOrEmpty(line)) // Chỉ lấy dòng có nội dung
                                    .ToList();

            var signIndex = contents.Count - 1;

            var contentIndex = 0;

            for (int index = 0; index < contents.Count; index++)
            {
                string text = contents[index].Trim();
                if (string.IsNullOrEmpty(text)) continue;

                var level = GetLevel(text);
                if (level > 0 && level != 8 && contentIndex == 0)
                {
                    contentIndex = index;
                } 
                if (level == 8)
                {
                    signIndex = index - 1;
                    break;
                }
            }

            var firstBlock = new DocumentBlock
            {
                Id = Guid.NewGuid(),
                Content = "",
                Title = "Mở đầu",
                ContentType = 1,
                Order = 0,
                ParentId = null,
                DocumentId = document.Id
            };

            var order = 1;

            for (int index = 0; index < contentIndex; index++)
            {
                string text = contents[index].Trim();
                if (string.IsNullOrEmpty(text)) continue;
                firstBlock.Content += firstBlock.Content.Length > 0 ? "\r\n" + text : text;
            }
            
            returnBlocks.Add(firstBlock);


            for (int index = contentIndex; index < signIndex; index++)
            {
                string text = contents[index].Trim();
                if (string.IsNullOrEmpty(text)) continue;
                var level = GetLevel(text);

                if (level > 0)
                {
                    var block = new DocumentBlock
                    {
                        Id = Guid.NewGuid(),
                        Content = text,
                        Title = text,
                        Level = level,
                        ContentType = 2,
                        Order = 2000 * order++,
                        ParentId = null,
                        DocumentId = document.Id
                    };
                    FindParent(block, blocks);
                    returnBlocks.Add(block);
                }
                else
                {
                    returnBlocks.Last().Content += "\r\n" + text;  
                }
            }


            blocks.Clear();


            var sightBlock = new DocumentBlock
            {
                Id = Guid.NewGuid(),
                Content = contents[signIndex],
                Title = "Chữ ký",
                ContentType = 3,
                Order = 2000 * order++,
                ParentId = null,
                DocumentId = document.Id,
                Level = 0,
                    
            };
            var sighName = ExtractSignerName(sightBlock.Content);
            document.SignerName = sighName;

            returnBlocks.Add(sightBlock);

            var (coquan, soluat, ngaybanhanh) = ExtractLawInfo(firstBlock.Content);
            document.Issuer = coquan;
            document.IssueDate = DateTime.ParseExact(ngaybanhanh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            document.DocumentNo = soluat;


            for (int index = signIndex + 1; index < contents.Count; index++)
            {
                string text = contents[index].Trim();
                if (string.IsNullOrEmpty(text)) continue;
                var level = GetLevel(text);
                if (level == 8)
                {
                    var block = new DocumentBlock
                    {
                        Id = Guid.NewGuid(),
                        Content = text,
                        Title = text,
                        Level = 0,
                        ContentType = 4,
                        Order = 2000 * order++,
                        ParentId = null,
                        DocumentId = document.Id
                    };
                    FindParent(block, blocks);
                    returnBlocks.Add(block);
                }
                else
                {
                    returnBlocks.Last().Content += "\r\n" + text;

                }
            }

            return returnBlocks;
        }

        private static int GetLevel(string content)
        {
            string dataCheck = content.Trim().Trim('*').Trim();

            string partRegex = @"^Phần\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, partRegex, RegexOptions.IgnoreCase)) return 1;

            string chapterRegex = @"^Chương\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, chapterRegex, RegexOptions.IgnoreCase)) return 2;

            string itemRegex = @"^Mục\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, itemRegex, RegexOptions.IgnoreCase)) return 3;

            string subsectionRegex = @"^Tiểu mục\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, subsectionRegex, RegexOptions.IgnoreCase)) return 4;

            string articleRegex = @"^Điều\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, articleRegex, RegexOptions.IgnoreCase)) return 5;

            string clauseRegex = @"^\d+(\.|\))";
            if (Regex.IsMatch(dataCheck, clauseRegex, RegexOptions.IgnoreCase)) return 6;

            string pointRegex = @"^\p{L}(\.|\))"; 
            if (Regex.IsMatch(dataCheck, pointRegex, RegexOptions.IgnoreCase)) return 7;

            string appendixRegex = @"^Phụ lục\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(dataCheck, appendixRegex, RegexOptions.IgnoreCase)) return 8;


            return 0;
        }

        private static void FindParent(DocumentBlock block, Stack<DocumentBlock> parentBlocks)
        {
            while (parentBlocks.Count() > 0 && block.Level <= parentBlocks.Peek().Level)
            {
                parentBlocks.Pop();
            }

            if (parentBlocks.Count() == 0)
            {
                block.ParentId = null;
            }
            else
            {
                block.ParentId = parentBlocks.Peek().Id;
            }
            parentBlocks.Push(block);
        }

       public async Task<IEnumerable<DocumentBlock>> GetBlockByDocumentId(Guid documentId)
        {
            return await _documentRepository.GetBlockByDocumentId(documentId);
        }
    }
}
