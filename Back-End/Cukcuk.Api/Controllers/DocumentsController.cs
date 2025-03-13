using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        private readonly IDocumentService _documentService = documentService;


        [HttpPost("upload-confirm")]
        public async Task<IActionResult> ConfirmUpload([FromQuery] Guid cacheId)
        {


            try
            {
                await _documentService.SaveFileUpload(cacheId);

                return StatusCode(200);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost("upload-cancel")]
        public async Task<IActionResult> CancelUpload([FromQuery] Guid cacheId)
        {


            try
            {
                await _documentService.CancelUpload(cacheId);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] List<IFormFile> files, [FromForm] Guid categoryId, [FromForm] Guid? parentId)
        {
            try
            {
                var response = await _documentService.UploadFile(files, categoryId, parentId);


                return StatusCode(200, response);
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(400, ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
                var data = await _documentService.GetFileDetailMarkdown(id);
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
