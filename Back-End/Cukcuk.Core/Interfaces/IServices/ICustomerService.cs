using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<string> GetNewCustomerCode();

        Task<object> FilterCustomer(int pageSize, int pageNumber, string? keyword);

        byte[] CreateExcelFile(List<Customer> employees);
        byte[] CreateResultFile(Guid? validCacheId, Guid InvalidCacheId);

        Task<object> ImportExcelFile(IFormFile file);

        Task AddDataImport(Guid cacheId);
    }
}
