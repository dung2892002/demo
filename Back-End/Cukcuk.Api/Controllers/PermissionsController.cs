using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PermissionsController(IPermissionService permissionService) : ControllerBase
    {
        private readonly IPermissionService _permissionService = permissionService;

        [HttpGet]
        public async Task<IActionResult> FindByName([FromQuery] string? name)
        {
            try
            {
                var permissions = await _permissionService.FindByName(name);
                return StatusCode(200, permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> FindByUser(string userId)
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionUsers(userId);
                return StatusCode(200, permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Permission permission)
        {
            try
            {
               await _permissionService.Create(permission);
                return StatusCode(201, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddPermissionToUser([FromBody] UserPermission permission)
        {
            try
            {
                await _permissionService.AddPermissionUser(permission);
                return StatusCode(201, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("delete-user-permission")]
        public async Task<IActionResult> DeletePermissionToUser([FromBody] UserPermission permission)
        {
            try
            {
                await _permissionService.DeletePermissionUser(permission);
                return StatusCode(200, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
