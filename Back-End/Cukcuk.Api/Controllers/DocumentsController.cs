using Cukcuk.Core.Entities;
using Cukcuk.Core.Enum;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        private readonly IDocumentService _documentService = documentService;

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
        public async Task<IActionResult> CreateFile([FromForm] IFormFile file, [FromForm] Guid? parentId, [FromForm] Guid categoryId)
        {
            try
            {
                await _documentService.CreateFile(file, parentId, categoryId);
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
    }
}
