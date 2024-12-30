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

        [Authorize(Roles = "Admin")]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterCustomer(int pageSize, int pageNumber, string? keyword)
        {
            try
            {
                var response = await _customerService.FilterCustomer(pageSize, pageNumber, keyword);

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

        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
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
