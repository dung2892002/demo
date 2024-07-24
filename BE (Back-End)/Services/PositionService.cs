using BE__Back_End_.Models;
using BE__Back_End_.Repositories.IRepo;
using BE__Back_End_.Services.IService;

namespace BE__Back_End_.Services
{
    public class PositionService(IPositionRepo positionRepo) : IPositionService
    {
        private readonly IPositionRepo _positionRepo = positionRepo;

        public async Task<IEnumerable<Position?>> GetPositions()
        {
            return await _positionRepo.FindAll();
        }

        public async Task<Position?> GetPositionById(Guid id)
        {
            return await _positionRepo.FindById(id);
        }

        public async Task CreatePosition(Position position)
        {
            position.PositionId = Guid.NewGuid();
            position.CreatedDate = DateTime.UtcNow;
            position.ModifiedDate = position.CreatedDate;

            await _positionRepo.Create(position);

        }

        public async Task UpdatePosition(Guid id, Position position)
        {
            var existingPosition = await _positionRepo.FindById(id);
            if (existingPosition == null)
            {
                throw new Exception("Position not exists");
            }

            position.PositionId = id;
            position.ModifiedDate = DateTime.UtcNow;

            await _positionRepo.Update(position);
        }

        public async Task DeletePosition(Guid id)
        {
            var existingPosition = await _positionRepo.FindById(id);
            if (existingPosition == null)
            {
                throw new Exception("Position not exists");
            }

            await _positionRepo.DeleteById(id);
        }
    }
}
