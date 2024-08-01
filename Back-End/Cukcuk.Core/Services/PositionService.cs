using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace Cukcuk.Core.Services
{
    public class PositionService(IPositionRepository positionRepository) : IPositionService
    {
        private readonly IPositionRepository _positionRepository = positionRepository;
        public async Task Create(Position position)
        {
            position.PositionId = Guid.NewGuid();
            position.CreatedDate = DateTime.UtcNow;
            position.ModifiedDate = position.CreatedDate;

            await _positionRepository.Create(position);
        }

        public async Task DeleteById(Guid id)
        {
            var existingPosition = await _positionRepository.FindById(id);
            if (existingPosition != null)
            {
                await _positionRepository.DeleteById(id);
            }
            else
            {
                throw new Exception("Position not exists");
            }
        }

        public async Task<IEnumerable<Position?>> GetAll()
        {
            return await _positionRepository.FindAll();
        }

        public async Task<Position?> GetById(Guid id)
        {
            return await _positionRepository.FindById(id);
        }

        public async Task Update(Guid id, Position position)
        {
            var existingPosition = await _positionRepository.FindById(id);
            if (existingPosition != null)
            {
                position.PositionId = id;
                position.ModifiedDate = DateTime.UtcNow;

                await _positionRepository.Update(position);
            }
            else
            {
                throw new Exception("Position not exists");
            }
        }
    }
}
