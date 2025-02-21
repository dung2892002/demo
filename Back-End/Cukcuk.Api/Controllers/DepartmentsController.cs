using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class DepartmentsController(IDepartmentService departmetnService) : ControllerBase
    {
        private readonly IDepartmentService _departmetnService = departmetnService;

        [HttpGet]
        public async Task<IActionResult?> GetDepartments()
        {
            try
            {
                var departments = await _departmetnService.GetAll();
                return StatusCode(200, departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult?> GetDepartment(Guid id)
        {
            try
            {
                var department = await _departmetnService.GetById(id);

                if (department == null)
                {
                    return StatusCode(404, "Department not exist");
                }

                return StatusCode(200, department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult?> CreateDepartment([FromBody] Department department)
        {
            try
            {
                await _departmetnService.Create(department);

                return StatusCode(201, "Created department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] Department department)
        {
            try
            {
                await _departmetnService.Update(id, department);
                return StatusCode(200, "Update department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            try
            {
                await _departmetnService.DeleteById(id);
                return StatusCode(200, "Delete department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
