using BE__Back_End_.Models;

namespace BE__Back_End_.Services.IService
{
    public interface IPositionService
    {
        Task<IEnumerable<Position?>> GetPositions();

        Task<Position?> GetPositionById(Guid id);

        Task CreatePosition(Position position);

        Task UpdatePosition(Guid id, Position position);

        Task DeletePosition(Guid id);
    }
}
