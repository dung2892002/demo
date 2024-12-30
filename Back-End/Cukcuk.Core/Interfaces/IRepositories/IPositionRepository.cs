using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.Repositories
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        Task<Position?> GetByName(string name);
    }
}
