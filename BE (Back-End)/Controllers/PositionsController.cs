using BE__Back_End_.Models;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BE__Back_End_.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors]
    public class PositionsController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public PositionsController(IDbConnection dbConnection) {
            _connection = dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            try
            {
                var query = "SELECT * FROM position";
                var positions = await _connection.QueryAsync<Position>(query);
                return StatusCode(200, positions);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPosition(Guid id)
        {
            try
            {
                var query = @"SELECT * FROM position WHERE PositionId=@Id";
                var position = await _connection.QuerySingleOrDefaultAsync<Position>(query, new { Id = id });

                if (position == null)
                {
                    return StatusCode(404, "Position not exist");
                }

                return StatusCode(200, position);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        } 

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] Position position)
        {
            try
            {
                position.PositionId = Guid.NewGuid();
                position.CreatedDate = DateTime.UtcNow;
                position.ModifiedDate = position.CreatedDate;

                var query = @"
                    INSERT INTO position (PositionId, PositionCode, PositionName, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                    VALUES (@PositionId, @PositionCode, @PositionName, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy);
                ";

                await _connection.ExecuteAsync(query, position);
                return StatusCode(201, "Created position succesfully");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] Position position)
        {
            try
            {
                var existingPosition = await _connection.QuerySingleOrDefaultAsync<Position>(
                    "SELECT * FROM position WHERE PositionId = @Id", new { Id = id });

                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }
                    

                position.PositionId = id;
                position.ModifiedDate = DateTime.UtcNow;

                var query = @"UPDATE position 
                            SET PositionName = @PositionName, 
                                ModifiedDate = @ModifiedDate, 
                                ModifiedBy = @ModifiedBy
                            WHERE PositionId = @PositionId";

                await _connection.ExecuteAsync(query, position);

                return StatusCode(200, "Update position succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePositon(Guid id)
        {
            try
            {
                var existingPosition = await _connection.QuerySingleOrDefaultAsync<Position>(
                    "SELECT * FROM position WHERE PositionId = @Id", new { Id = id });

                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }

                var query = "Delete from position where PositionId=@Id";
                await _connection.ExecuteAsync(query, new { Id = id });

                return StatusCode(200, "Delete Position succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
