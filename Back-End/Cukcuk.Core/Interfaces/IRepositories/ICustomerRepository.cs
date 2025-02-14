using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<string> GennerateNewCustomerCode();
        Task<(IEnumerable<Customer> customers, int TotalCount)> FilterCustomers(
        int pageSize, int pageNumber, string? keyword, Guid? groupId);
        Task<bool> CheckCustomerCode(string customerCode);
        Task<bool> CheckMobileNumber(string mobileNumber);
    }
}
