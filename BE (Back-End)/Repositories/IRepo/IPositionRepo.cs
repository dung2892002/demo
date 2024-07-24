using BE__Back_End_.Models;

namespace BE__Back_End_.Repositories.IRepo
{
    public interface IPositionRepo
    {
        Task<IEnumerable<Position>> FindAll();

        Task<Position> FindById(Guid id);

        Task Create(Position position);

        Task Update(Position position);

        Task DeleteById(Guid id);
    }
}
