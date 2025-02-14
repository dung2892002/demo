using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FoldersController(IFolderRepository folderRepository) : ControllerBase
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        [HttpGet]
        public async Task<IActionResult> GetByMenu([FromQuery] Guid menuId)
        {
            try
            {
                var folders = await _folderRepository.GetByMenuId(menuId);
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var folder = await _folderRepository.GetFolderById(id);
                return Ok(folder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Folder folder)
        {
            try
            {
                folder.Id = Guid.NewGuid();
                await _folderRepository.Create(folder);
                return StatusCode(201, folder);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("subs")]
        public async Task<IActionResult> GetSubFolders([FromQuery] Guid parentId)
        {
            try
            {
                var folders = await _folderRepository.GetSubFolders(parentId);
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFolder([FromQuery] Guid id)
        {
            try
            {
                await _folderRepository.DeleteFolder(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFolder(Folder folder)
        {
            try
            {
                await _folderRepository.UpdateFolder(folder);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
