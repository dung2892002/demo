using BE__Back_End_.Models;
using BE__Back_End_.Services.IService;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BE__Back_End_.Controllers
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
                var departments = await _departmetnService.GetDepartments();
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
                var department = await _departmetnService.GetDepartmentById(id);

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
                await _departmetnService.CreateDepartment(department);

                return StatusCode(201, "Created department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id,  [FromBody] Department department)
        {
            try
            {
                await _departmetnService.UpdateDepartment(id, department);
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
                await _departmetnService.DeleteDepartment(id);
                return StatusCode(200, "Delete department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
