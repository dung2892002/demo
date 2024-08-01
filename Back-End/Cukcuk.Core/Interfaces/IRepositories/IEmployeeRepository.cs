using Cukcuk.Core.DTOs;

namespace Cukcuk.Core.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<EmployeeDTO>
    {
        Task<string> GennerateNewEmployeeCode();
        Task<(IEnumerable<EmployeeDTO> Employees, int TotalCount)> FilterEmployees(
        int pageSize, int offset, string? employeeFilter, Guid? departmentId, Guid? positionId);
    }
}
