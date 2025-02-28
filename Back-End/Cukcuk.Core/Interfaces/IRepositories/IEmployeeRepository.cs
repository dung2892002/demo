using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<EmployeeDTO>
    {
        Task<string> GennerateNewEmployeeCode();
        Task<(IEnumerable<EmployeeDTO> Employees, int TotalCount)> FilterEmployees(
        int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId);
        Task<bool> CheckEmployeeCode(string employeeCode);
        Task<bool> CheckMobileNumber(string mobileNumber);

        Task<PageResult<EmployeeFolder>> GetFolder(Guid? parentId, string? keyword, int pageSize, int pageNumber, bool? sortName, bool? sortDate, bool? sortType);
        Task CreateFolder(EmployeeFolder folder);


        Task<IEnumerable<EmployeeFolder>> GetFolderOnly();
    }
}
