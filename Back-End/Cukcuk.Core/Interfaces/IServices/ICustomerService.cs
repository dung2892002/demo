using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<string> GetNewCustomerCode();
        Task<object> FilterCustomer(int pageSize, int pageNumber, string? keyword, Guid? groupId);

        byte[] CreateExcelFile(List<Customer> employees);
        byte[] CreateResultFile(Guid? validCacheId, Guid InvalidCacheId);

        Task<object> ImportExcelFile(IFormFile file);

        Task AddDataImport(Guid cacheId);

        Task<IEnumerable<CustomerGroup?>> GetGroups();

        Task<IEnumerable<CustomerFolder>> GetFolder(Guid? parentId, bool? sortName, bool? sortDate, bool? sortType);

        Task CreateFolder(CustomerFolder folder);
    }
}
