using Cukcuk.Core.DTOs;
using Cukcuk.Core.Helper;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<EmployeeDTO>
    {
        Task<string> GetNewEmployeeCode();

        Task<IEnumerable<EmployeeDTO>> GetFileContent(Guid fileId);
        Task<object> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId);

        byte[] CreateExcelFile(List<EmployeeDTO> employees);
        byte[] CreateResultFile(Guid? validCacheId, Guid InvalidCacheId);

        Task<object> ImportExcelFile(IFormFile file);

        Task AddDataImport(Guid cacheId);
    }
}
