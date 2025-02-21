using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.Services
{
    public interface IPositionService : IBaseService<Position>
    {
        Task<Position?> GetByName(string name);
    }
}
