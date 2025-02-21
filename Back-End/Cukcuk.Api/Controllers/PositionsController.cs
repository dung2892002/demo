using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class PositionsController(IPositionService positionService) : ControllerBase
    {
        private readonly IPositionService _positionService = positionService;
        [HttpGet]
        public async Task<IActionResult?> GetPositions()
        {
            try
            {
                var positions = await _positionService.GetAll();
                return StatusCode(200, positions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult?> GetPosition(Guid id)
        {
            try
            {
                var position = await _positionService.GetById(id);

                if (position == null)
                {
                    return StatusCode(404, "Position not exist");
                }

                return StatusCode(200, position);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult?> CreatePosition([FromBody] Position position)
        {
            try
            {
                await _positionService.Create(position);
                return StatusCode(201, "Created position succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult?> UpdatePosition(Guid id, [FromBody] Position position)
        {
            try
            {
                await _positionService.Update(id, position);
                return StatusCode(200, "Update position succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult?> DeletePositon(Guid id)
        {
            try
            {
                await _positionService.DeleteById(id);

                return StatusCode(200, "Delete Position succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
