using BE__Back_End_.Payloads.DTOs;
using BE__Back_End_.Payloads.Request;

namespace BE__Back_End_.Services.IService
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponse>> GetEmployees();

        Task<EmployeeResponse> GetEmployeeById(Guid id);

        Task CreateEmployee(EmployeeRequest employeeRequest);

        Task UpdateEmployee(Guid id, EmployeeRequest employeeRequest);

        Task DeleteEmployee(Guid id);

        Task<string> GetNewEmployeeCode();

        Task<object> FilterEmployees(
            int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId);
    }
}
