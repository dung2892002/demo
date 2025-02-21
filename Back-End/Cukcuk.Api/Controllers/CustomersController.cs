using Cukcuk.Core.Auth;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cukcuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;


        [HttpGet("folder")]
        public async Task<IActionResult> GetByFolder([FromQuery] Guid? parentId, [FromQuery] bool? sortName, [FromQuery] bool? sortDate, [FromQuery] bool? sortType)
        {
            try
            {
                var results = await _customerService.GetFolder(parentId, sortName, sortDate, sortType);

                return StatusCode(200, results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("folder")]

        public async Task<IActionResult> CreateFolder([FromBody] CustomerFolder folder)
        {
            try
            {
                await _customerService.CreateFolder(folder);

                return StatusCode(201, "success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin, CustomerManager")]
        [HttpGet("filter")]

        public async Task<IActionResult> FilterCustomer(int pageSize, int pageNumber, string? keyword, Guid? groupId)
        {
            try
            {
                var response = await _customerService.FilterCustomer(pageSize, pageNumber, keyword, groupId);

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            try
            {

                var employee = await _customerService.GetById(id);
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

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var response = await _customerService.GetGroups();

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("export")]
        public IActionResult ExportFile([FromBody] List<Customer> datas)
        {
            try
            {
                var file = _customerService.CreateExcelFile(datas);

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export_data.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, CustomerManager")]
        [HttpPost]
        [Permission("AddCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                await _customerService.Create(customer);
                return StatusCode(201, "Created customer succesfully");
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

        [Authorize(Roles = "Admin, CustomerManager")]
        [HttpPut("{id}")]
        [Permission("EditCustomer")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Customer customer)
        {
            try
            {
                await _customerService.Update(id, customer);
                return StatusCode(200, "Update successfully");
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

        [Authorize(Roles = "Admin, CustomerManager")]
        [HttpDelete("{id}")]
        [Permission("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {

                await _customerService.DeleteById(id);

                return StatusCode(200, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, CustomerManager")]
        [HttpPost("import")]
        [Permission("ImportCustomer")]
        public async Task<IActionResult> ImportFile([FromForm] List<IFormFile> file)
        {
            try
            {
                var response = await _customerService.ImportExcelFile(file[0]);
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
                await _customerService.AddDataImport(cacheId);
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
                var file = _customerService.CreateResultFile(validCacheId, invalidCacheId);

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
