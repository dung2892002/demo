using Cukcuk.Core.DTOs;
using Cukcuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, IPositionService positionService) : ControllerBase
    {
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IPositionService _positionService = positionService;
        private readonly IDepartmentService _departmentService = departmentService;

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAll();
                return StatusCode(200, employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            try
            {

                var employee = await _employeeService.GetById(id);
                if (employee == null)
                {
                    return StatusCode(404, "Employee not exist");
                }

                return StatusCode(200, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO.PositionId == Guid.Empty || employeeDTO.DepartmentId == Guid.Empty)
                {
                    return StatusCode(400, "PositionId and DepartmentId are required");
                }

                var existingDepartment = await _departmentService.GetById(employeeDTO.DepartmentId);
                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                var existingPosition = await _positionService.GetById(employeeDTO.PositionId);
                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }

                await _employeeService.Create(employeeDTO);

                return StatusCode(201, "Created employee succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO.PositionId == Guid.Empty || employeeDTO.DepartmentId == Guid.Empty)
                {
                    return StatusCode(400, "PositionId and DepartmentId are required");
                }

                var existingDepartment = await _departmentService.GetById(employeeDTO.DepartmentId);
                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                var existingPosition = await _positionService.GetById(employeeDTO.PositionId);
                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }

                var existingEmployee = await _employeeService.GetById(id);
                if (existingEmployee == null)
                {
                    return StatusCode(404, "Employee not exists");
                }

                await _employeeService.Update(id, employeeDTO);

                return StatusCode(200, "Update employee successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var existingEmployee = await _employeeService.GetById(id);
                if (existingEmployee == null)
                {
                    return StatusCode(404, "Employee not exists");
                }

                await _employeeService.DeleteById(id);

                return StatusCode(200, "Delete employee successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("NewEmployeeCode")]
        public async Task<IActionResult> GetNewEmployeeCode()
        {
            try
            {
                var newEmployeeCode = await _employeeService.GetNewEmployeeCode();
                return StatusCode(200, newEmployeeCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterEmployee(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
        {
            try
            {
                var response = await _employeeService.FilterEmployees(pageSize, pageNumber, employeeFilter, departmentId, positionId);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
