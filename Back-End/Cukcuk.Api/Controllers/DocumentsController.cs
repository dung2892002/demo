using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;
using System.Text.RegularExpressions;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        private readonly IDocumentService _documentService = documentService;


        [HttpPost("upload")]
        public async Task<IActionResult> TestUpload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file.");

            string filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            List<DocumentBlock> blocks = SplitWordDocument(filePath);

            return Ok(blocks);
        }

        private static List<DocumentBlock> SplitWordDocument(string filePath)
        {
            Stack<DocumentBlock> blocks = new Stack<DocumentBlock>();
            List<DocumentBlock> returnBlocks = new List<DocumentBlock>();
            Aspose.Words.Document doc = new Aspose.Words.Document(filePath);

            var firstBlock = new DocumentBlock
            {
                Id = Guid.NewGuid(),
                Content = "",
                Title = "Mở đầu",
                ContentType = 1,
                ParentId = null,
            };

           
            foreach (Aspose.Words.Paragraph para in doc.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
            {
                string text = para.GetText().Trim();
                if (string.IsNullOrEmpty(text)) continue;
                var level = GetLevel(text);

                if (level > 0 )
                {
                    var block = new DocumentBlock
                    {
                        Id = Guid.NewGuid(),
                        Content = text,
                        Title = text,
                        Level = level,
                        ContentType = 2,
                        ParentId = null
                    };

                    FindParent(block, blocks, returnBlocks);
                }
                else
                {
                    if (blocks.Count > 0)
                    {
                        var preBlock = blocks.Pop();
                        preBlock.Content += text;
                        blocks.Push(preBlock);
                    }
                    else
                    {
                        firstBlock.Content += text;
                    }
                }
            }

            returnBlocks.Insert(0, firstBlock);

            return returnBlocks;
        }


        private static int GetLevel(string content)
        {
            string partRegex = @"^Phần\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(content.Trim(), partRegex, RegexOptions.IgnoreCase)) return 1;

            string chapterRegex = @"^Chương\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(content.Trim(), chapterRegex, RegexOptions.IgnoreCase)) return 2;

            string itemRegex = @"^Mục\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(content.Trim(), itemRegex, RegexOptions.IgnoreCase)) return 3;

            string subsectionRegex = @"^Tiểu mục\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(content.Trim(), subsectionRegex, RegexOptions.IgnoreCase)) return 4;

            string articleRegex = @"^Điều\s+([IVXLCDM\d]+)\.?";
            if (Regex.IsMatch(content.Trim(), articleRegex, RegexOptions.IgnoreCase)) return 5;

            string clauseRegex = @"^\d+(\.|\))";
            if (Regex.IsMatch(content.Trim(), clauseRegex, RegexOptions.IgnoreCase)) return 6;

            string pointRegex = @"^[a-zA-Z](\.|\))";
            if (Regex.IsMatch(content.Trim(), pointRegex, RegexOptions.IgnoreCase)) return 7;

            return 0;
        }

        private static void FindParent(DocumentBlock block, Stack<DocumentBlock> parentBlocks, List<DocumentBlock> returnBlocks)
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
            returnBlocks.Add(block);
            parentBlocks.Push(block);
        }

        [HttpGet("parents")]
        public async Task<IActionResult> FilterDocuments([FromQuery] Guid id)
        {
            try
            {
                var results = await _documentService.GetParentDocuments(id);
                return Ok(results);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterDocuments([FromQuery] Guid? parentId, [FromQuery] string? keyword, [FromQuery] int pageSize,
            [FromQuery] int pageNumber, [FromQuery] Guid? categoryId, [FromQuery] DocumentType? type)
        {
            try
            {
                var results = await _documentService.FilterDocument(parentId, keyword, pageSize, pageNumber, categoryId, type);
                return Ok(results);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("folder")]
        public async Task<IActionResult> Create([FromBody] Document document)
        {
            try
            {
                await _documentService.Create(document);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("file")]
        public async Task<IActionResult> CreateFile([FromForm] List<IFormFile> files, [FromForm] Guid? parentId, [FromForm] Guid categoryId)
        {
            try
            {
                await _documentService.CreateFile(files, parentId, categoryId);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            try
            {
                await _documentService.Delete(id);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategoryDocument()
        {
            try
            {
                var categories = await _documentService.FindAllCategory();
                return StatusCode(200, categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Document document)
        {
            try
            {
                await _documentService.Update(id, document);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _documentService.GetById(id);
                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("content/{id}")]
        public async Task<IActionResult> GetHtmlData(Guid id)
        {
            try
            {
                var data = await _documentService.GetFileDetailHtml(id);
                return Ok(data);
            }
            catch (FileNotFoundException)
            {
                return NotFound("File không tồn tại.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("subs")]
        public async Task<IActionResult> GetSubsDocumentFolder([FromQuery] Guid? id)
        {
            try
            {
                var folders = await _documentService.GetSubsDocumentFolder(id);
                return StatusCode(200, folders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPatch("move/{id}")]
        public async Task<IActionResult> MoveDocument(Guid id, [FromForm] Guid? parentId)
        {
            try
            {
                await _documentService.MoveDocument(id, parentId);
                return StatusCode(200);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportListDocument([FromQuery] Guid? folderId, [FromQuery] string? keyword)
        {
            try
            {
                var file = await _documentService.CreateExcelFile(folderId, keyword);

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách tài liệu.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> DeleteRange([FromBody] List<Guid> ids)
        {
            try
            {
                await _documentService.DeleteRange(ids);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("move")]
        public async Task<IActionResult> Moverange([FromQuery] Guid? parentId, [FromBody] List<Guid> ids)
        {
            try
            {
                await _documentService.MoveRange(ids, parentId);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
