﻿using Cukcuk.Core.Auth;
using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class EmployeesController(IEmployeeService employeeService, IEmployeeRepository employeeRepository) : ControllerBase
    {
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;


        //[HttpPost("test")]
        //public async Task<IActionResult> Test()
        //{
        //    var employees = await _employeeRepository.FindAll();
        //    var folders = (await _employeeRepository.GetFolderOnly()).ToList();

        //    var random = new Random();

        //    foreach (var employee in employees)
        //    {
        //        var employeeFolder = new EmployeeFolder
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = employee.Fullname,
        //            CreatedAt = DateTime.Now,
        //            EmployeeId = employee.EmployeeId,
        //            Type = false,
        //            ParentId = folders.Count > 0 ? folders[random.Next(folders.Count)].Id : (Guid?)null
        //        };

        //        await _employeeRepository.CreateFolder(employeeFolder);
        //    }
        //    return StatusCode(200);
        //}

        [HttpGet("folder")]
        public async Task<IActionResult> GetByFolder([FromQuery] Guid? parentId, [FromQuery] bool? sortName, [FromQuery] bool? sortDate, [FromQuery] bool? sortType,
                                                    [FromQuery] string? keyword, [FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            try
            {
                var results = await _employeeService.GetFolder(parentId, keyword, pageSize, pageNumber, sortName, sortDate, sortType);

                return StatusCode(200, results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("folder")]

        public async Task<IActionResult> CreateFolder([FromBody] EmployeeFolder folder)
        {
            try
            {
                await _employeeService.CreateFolder(folder);

                return StatusCode(201, "success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize]
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
                await _employeeService.Create(employeeDTO);

                return StatusCode(201, "Created employee succesfully");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeDTO employeeDTO)
        {
            try
            {
                await _employeeService.Update(id, employeeDTO);

                return StatusCode(200, "Update employee successfully");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, EmployeeManager")]
        [HttpDelete("{id}")]
        [Permission("DeleteEmployee")]
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

        [Authorize(Roles = "Admin, EmployeeManager")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("export")]
        public IActionResult ExportFile([FromBody] List<EmployeeDTO> datas)
        {
            try
            {
                var file = _employeeService.CreateExcelFile(datas);

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export_data.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, EmployeeManager")]
        [HttpPost("import")]
        [Permission("ImportEmployee")]
        public async Task<IActionResult> ImportFile([FromForm] List<IFormFile> file)
        {
            try
            {
                var response = await _employeeService.ImportExcelFile(file[0]);
                return StatusCode(200, response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (FormatException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import/{cacheId}")]
        public async Task<IActionResult> AddDataImport(Guid cacheId)
        {
            try
            {
                await _employeeService.AddDataImport(cacheId);
                return StatusCode(201, "success");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import/result")]
        public IActionResult GetReport([FromQuery] Guid invalidCacheId, [FromQuery] Guid? validCacheId)
        {
            try
            {
                var file = _employeeService.CreateResultFile(validCacheId, invalidCacheId);

                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data_import_result.xlsx");
               
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
