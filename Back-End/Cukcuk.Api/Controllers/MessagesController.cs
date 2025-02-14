using Cukcuk.Core.DTOs;
using Cukcuk.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MessagesController(IMessageRepository messageRepository) : ControllerBase
    {
        private readonly IMessageRepository _repo = messageRepository;

        [HttpGet]
        public async Task<IActionResult> GetMessage([FromQuery] string userId1,[FromQuery] string userId2, [FromQuery] int pageNumber, [FromQuery] int skipItem)
        {
            try
            {
                var (messages, totalPages) = await _repo.GetConversation(userId1, userId2, pageNumber, skipItem);
                return StatusCode(200, new
                {
                    Messages = messages,
                    TotalPages = totalPages,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserToChat([FromQuery] string userId)
        {
            try
            {
                var user = await _repo.GetUserToChat(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> GetUserToChat(Guid id)
        {
            try
            {
                await _repo.ReadMessage(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
