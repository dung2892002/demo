using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportsController(IImportService importService) : ControllerBase
    {
        private readonly IImportService _importService = importService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var imports = await _importService.GetAll();
                return StatusCode(200, imports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Import import)
        {
            try
            {
                Console.WriteLine("post controller");
                await _importService.Create(import);
                return StatusCode(201, "success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Import import, int id)
        {
            try
            {
                await _importService.UpdateInt(id ,import);
                return StatusCode(200, "success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                await _importService.DeleteInt(id);
                return StatusCode(200, "success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
