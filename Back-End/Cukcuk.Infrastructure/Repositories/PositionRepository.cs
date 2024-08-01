using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Dapper;
using System.Data;

namespace Cukcuk.Infrastructure.Repositories
{
    public class PositionRepository(IDbConnection connection) : IPositionRepository
    {
        private readonly IDbConnection _connection = connection;
        public async Task Create(Position position)
        {
            var query = @"
                INSERT INTO position (PositionId, PositionCode, PositionName, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                VALUES (@PositionId, @PositionCode, @PositionName, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy);
            ";

            await _connection.ExecuteAsync(query, position);
        }

        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM position WHERE PositionId=@Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<IEnumerable<Position?>> FindAll()
        {
            var query = "SELECT * FROM position";
            return await _connection.QueryAsync<Position>(query);
        }

        public async Task<Position?> FindById(Guid id)
        {
            var query = @"SELECT * FROM position WHERE PositionId=@Id";
            return await _connection.QuerySingleOrDefaultAsync<Position>(query, new { Id = id });
        }

        public async Task Update(Position position)
        {
            var query = @"UPDATE position 
                        SET PositionName = @PositionName, 
                            ModifiedDate = @ModifiedDate, 
                            ModifiedBy = @ModifiedBy
                       WHERE PositionId = @PositionId";

            await _connection.ExecuteAsync(query, position);
        }
    }
}
