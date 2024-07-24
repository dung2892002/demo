using BE__Back_End_.Models;
using BE__Back_End_.Repositories.IRepo;
using Dapper;
using System.Data;

namespace BE__Back_End_.Repositories
{
    public class PositionRepo : IPositionRepo
    {   

        private readonly IDbConnection _connection;

        public PositionRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Position>> FindAll()
        {
            var query = "SELECT * FROM position";
            return await _connection.QueryAsync<Position>(query);
        }

        public async Task<Position> FindById(Guid id)
        {
            var query = @"SELECT * FROM position WHERE PositionId=@Id";
            return await _connection.QuerySingleOrDefaultAsync<Position>(query, new { Id = id });

        }

        public async Task Create(Position position)
        {
            var query = @"
                INSERT INTO position (PositionId, PositionCode, PositionName, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                VALUES (@PositionId, @PositionCode, @PositionName, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy);
            ";

            await _connection.ExecuteAsync(query, position);
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

        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM position WHERE PositionId=@Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
