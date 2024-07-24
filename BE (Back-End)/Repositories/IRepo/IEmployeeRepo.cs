using BE__Back_End_.Models;
using BE__Back_End_.Payloads.DTOs;
using BE__Back_End_.Payloads.Request;

namespace BE__Back_End_.Repositories.IRepo
{
    public interface IEmployeeRepo
    {
        Task<IEnumerable<EmployeeResponse>> FindAll();

        Task<EmployeeResponse> FindById(Guid id);

        Task Create(EmployeeRequest employeeRequest);

        Task Update(EmployeeRequest employeeRequest);

        Task DeleteById(Guid id);

        Task<string> GennerateNewEmployeeCode();

        Task<(IEnumerable<EmployeeResponse> Employees, int TotalCount)> FilterEmployees(
        int pageSize, int offset, string? employeeFilter, Guid? departmentId, Guid? positionId);
    }
}
