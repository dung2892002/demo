using Cukcuk.Core.DTOs;

namespace Cukcuk.Core.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<EmployeeDTO>
    {
        Task<string> GetNewEmployeeCode();

        Task<object> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId);
    }
}
