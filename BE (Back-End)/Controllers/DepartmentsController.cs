using BE__Back_End_.Models;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BE__Back_End_.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public DepartmentsController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> getDepartments()
        {
            try
            {
                var query = "SELECT * FROM department";
                var departments = await _connection.QueryAsync<Department>(query);
                return StatusCode(200, departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getDepartment(Guid id)
        {
            try
            {
                var query = @"SELECT * FROM department WHERE DepartmentId=@Id";
                var department = await _connection.QuerySingleOrDefaultAsync<Department>(query, new { Id = id });

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
        public async Task<IActionResult> createDepartment([FromBody] Department department)
        {
            try
            {
                department.DepartmentId = Guid.NewGuid();
                department.CreatedDate = DateTime.UtcNow;
                department.ModifiedDate = department.CreatedDate;

                var query = @"
                    INSERT INTO department (DepartmentId, DepartmentCode, DepartmentName, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                    VALUES (@DepartmentId, @DepartmentCode, @DepartmentName, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy);
                ";

                await _connection.ExecuteAsync(query, department);

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
                var existingDepartment = await _connection.QuerySingleOrDefaultAsync<Position>(
                   "SELECT * FROM department WHERE DepartmentId = @Id", new { Id = id });

                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                department.DepartmentId = id;
                department.ModifiedDate = DateTime.UtcNow;

                var query = @"UPDATE department 
                            SET DepartmentName = @DepartmentName, 
                                ModifiedDate = @ModifiedDate, 
                                ModifiedBy = @ModifiedBy
                            WHERE DepartmentId = @DepartmentId";

                await _connection.ExecuteAsync(query, department);

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
                var existingDepartment = await _connection.QuerySingleOrDefaultAsync<Position>(
                    "SELECT * FROM department WHERE DepartmentId = @Id", new { Id = id });

                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                var query = "Delete from department where DepartmentId=@Id";
                await _connection.ExecuteAsync(query, new { Id = id });

                return StatusCode(200, "Delete department succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
