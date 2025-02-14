using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService) : ControllerBase
    {
        private readonly IFileService _fileService = fileService;

        [HttpGet]
        public async Task<IActionResult> GetFileByFolder([FromQuery] Guid folderId)
        {
            try
            {
                var files = await _fileService.GetByFoler(folderId);
                return StatusCode(200, files);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpGet("url/{fileId}")]
        public async Task<IActionResult> GetUrl(Guid fileId)
        {
            try
            {
                var url = await _fileService.GetFileUrl(fileId);
                return StatusCode(200, url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromForm] Guid folderId ,[FromForm] List<IFormFile> files)
        {
            try
            {
                var file = await _fileService.Create(folderId, files);
                return StatusCode(201, file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteFile([FromQuery] Guid id)
        {
            try
            {
                await _fileService.Delete(id);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
