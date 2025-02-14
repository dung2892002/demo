using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Dapper;
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

        public Task<IEnumerable<Customer?>> FindAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Customer?> FindById(Guid? id)
        {
            var query = @"
                SELECT 
                    c.*, 
                    g.GroupId, g.GroupName
                FROM 
                    customer c
                    LEFT JOIN customergroup g ON c.GroupId = g.GroupId
                WHERE c.CustomerId = @Id
            ";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(query, new { Id = id });
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
        }

    }
}
