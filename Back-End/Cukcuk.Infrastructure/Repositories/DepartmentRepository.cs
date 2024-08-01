using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using System.Data;
using Dapper;

namespace Cukcuk.Infrastructure.Repositories
{
    public class DepartmentRepository(IDbConnection connection) : IDepartmentRepository
    {
        private readonly IDbConnection _connection = connection;

        public async Task Create(Department department)
        {
            var query = @"
                INSERT INTO department (DepartmentId, DepartmentCode, DepartmentName, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                VALUES (@DepartmentId, @DepartmentCode, @DepartmentName, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy);
            ";
            await _connection.ExecuteAsync(query, department);
        }

        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM department WHERE DepartmentId=@Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<IEnumerable<Department?>> FindAll()
        {
            var query = "SELECT * FROM department";
            return await _connection.QueryAsync<Department>(query);
        }

        public async Task<Department?> FindById(Guid id)
        {
            var query = "SELECT * FROM department WHERE DepartmentId=@Id";
            return await _connection.QuerySingleOrDefaultAsync<Department>(query, new { Id = id });
        }

        public async Task Update(Department department)
        {
            var query = @"UPDATE department 
                        SET DepartmentName = @DepartmentName, 
                            ModifiedDate = @ModifiedDate, 
                            ModifiedBy = @ModifiedBy
                        WHERE DepartmentId = @DepartmentId";
            await _connection.ExecuteAsync(query, department);
        }
    }
}
