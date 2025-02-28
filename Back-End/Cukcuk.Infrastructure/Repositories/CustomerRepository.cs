using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Dapper;
using MathNet.Numerics.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cukcuk.Infrastructure.Repositories
{
    public class CustomerRepository(IDbConnection connection, ApplicationDbContext dbContext) : ICustomerRepository
    {
        private readonly IDbConnection _connection = connection;
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task<bool> CheckCustomerCode(string customerCode)
        {
            var result = await _dbContext.Customers.AnyAsync(c => c.CustomerCode == customerCode);

            return result;
        }

        public async Task<bool> CheckMobileNumber(string mobileNumber)
        {
            var result = await _dbContext.Customers.AnyAsync(c => c.MobileNumber == mobileNumber);

            return result;
        }

        public async Task Create(Customer entity)
        {
            var query = @"
                INSERT INTO customer (
                    CustomerId, CustomerCode, Fullname, DateOfBirth, Gender, Address, MobileNumber, Email, Amount, GroupId
                )
                VALUES (
                    @CustomerId, @CustomerCode, @Fullname, @DateOfBirth, @Gender, @Address, @MobileNumber,@Email,
                    @Amount, @GroupId
                )
            ";
            await _connection.ExecuteAsync(query, entity);
        }


        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM customer WHERE CustomerId = @Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<(IEnumerable<Customer> customers, int TotalCount)> FilterCustomers(int pageSize, int pageNumber, string? keyword, Guid? groupId)
        {
            int offset = (pageNumber - 1) * pageSize;
            var query = @"
                SELECT SQL_CALC_FOUND_ROWS
                    c.*, 
                    cg.GroupId, cg.GroupName 
                FROM 
                    customer c
                    LEFT JOIN customergroup cg ON c.GroupId = cg.GroupId
                WHERE 1 = 1";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " AND (c.CustomerCode LIKE CONCAT('%', @Keyword, '%') " +
                         "OR c.Fullname LIKE CONCAT('%', @Keyword, '%'))";
            }

            if (groupId != null)
            {
                query += " AND (c.GroupId = @groupId)";
            }
            query += " ORDER BY c.CustomerCode DESC LIMIT @PageSize OFFSET @Offset;";
            query += " SELECT FOUND_ROWS() AS TotalCount;";

            var parameters = new
            {
                PageSize = pageSize,
                Offset = offset,
                Keyword = keyword,
                GroupId = groupId
            };

            var multi = await _connection.QueryMultipleAsync(query, parameters);
            var customers = await multi.ReadAsync<Customer>();
            var totalRecords = await multi.ReadSingleAsync<int>();

            return (customers, totalRecords);
        }

        public async Task<IEnumerable<Customer?>> FindAll()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer?> FindById(Guid? id)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<string> GennerateNewCustomerCode()
        {
            var query = @"
                SELECT CustomerCode
                FROM customer
                ORDER BY CAST(SUBSTRING(CustomerCode, 4) AS UNSIGNED) DESC
                LIMIT 1;
            ";

            var lastCustomerCode = await _connection.QueryFirstOrDefaultAsync<string>(query);
            if (lastCustomerCode == null)
            {
                return "KH-00001";
            }

            int lastNumber = int.Parse(lastCustomerCode[3..]);
            int newNumber = lastNumber + 1;
            return "KH-" + newNumber.ToString("D5");
        }

        public async Task<PageResult<CustomerFolder>> GetFolder(Guid? parentId, string? keyword, int pageSize, int pageNumber, bool? sortName, bool? sortDate, bool? sortType)
        {
            var query = _dbContext.CustomFolders.AsNoTracking().AsQueryable();

            query = query.Where(cf => parentId.HasValue ? cf.ParentId == parentId.Value : cf.ParentId == null);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(cf => cf.Name.Contains(keyword));
            }

            var totalItems = await query.CountAsync();

            if (sortName.HasValue)
            {
               query = sortName.Value ? query.OrderBy(cf => cf.Name) : query.OrderByDescending(cf => cf.Name);
            }

            if (sortDate.HasValue)
            {
                query = sortDate.Value ? query.OrderBy(cf => cf.CreatedAt) : query.OrderByDescending(cf => cf.CreatedAt);
            }

            if (sortType.HasValue)
            {
                query = sortType.Value ? query.OrderBy(cf => cf.Type) : query.OrderByDescending(cf => cf.Type);
            }

            var customerFoldes = await query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Include(c => c.Customer)
                .ToListAsync();

            return new PageResult<CustomerFolder>
            {
                Items = customerFoldes,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };
        }

        public async Task Update(Customer entity)
        {

            var query = @"
                UPDATE customer SET
                    Fullname = @Fullname,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    Address = @Address,
                    MobileNumber = @MobileNumber,
                    Email = @Email,
                    GroupId = @GroupId
                WHERE CustomerId = @CustomerId";
            await _connection.ExecuteAsync(query, entity);
            var query2 = @"UPDATE customer_folder SET NAME = @Fullname WHERE CustomerId = @CustomerId";
            await _connection.ExecuteAsync(query2, entity);
        }

        public async Task CreateFolder(CustomerFolder folder)
        {
            await _dbContext.CustomFolders.AddAsync(folder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerFolder>> GetFolderOnly()
        {
            return await _dbContext.CustomFolders.Where(cf => cf.Type == true).ToListAsync();
        }

        public async Task<StatisticalGenderResponse> StatisticalGenderCustomer()
        {
            var result = await _dbContext.Customers.AsNoTracking()
                .GroupBy(c => c.Gender)
                .Select(g => new
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            int totalMale = result.FirstOrDefault(s => s.Gender == 1)?.Count ?? 0;
            int totalFemale = result.FirstOrDefault(s => s.Gender == 0)?.Count ?? 0;
            int total = result.Sum(s => s.Count);
            int totalUnknown = total - totalMale - totalFemale;

            return new StatisticalGenderResponse
            {
                Total = total,
                TotalMale = totalMale,
                TotalFemale = totalFemale,
                TotalUnknown = totalUnknown
            };
        }

        public async Task<IEnumerable<StatisticalDobResponse>> StatisticalDobCustomer()
        {
            var result = await _dbContext.Customers.AsNoTracking()
                .GroupBy(c => c.DateOfBirth.Value.Year)
                .OrderBy(g  => g.Key)
                .Select(g => new StatisticalDobResponse
                    {
                        YearValue = g.Key,
                        CountValue = g.Count()
                    })
                .ToListAsync();

            return result;
        }
    }
}
