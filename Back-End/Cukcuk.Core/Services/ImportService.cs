using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.IServices;

namespace Cukcuk.Core.Services
{
    public class ImportService(IImportRepository importRepository) : IImportService
    {
        private readonly IImportRepository _importRepository = importRepository;
        public async Task Create(Import entity)
        {
            entity.Id = null;
            await _importRepository.Create(entity);
        }

        public async Task DeleteById(Guid id)
        {
            await _importRepository.DeleteById(id);
        }

        public async Task DeleteInt(int id)
        {
            await _importRepository.DeleteByIdInt(id);
        }

        public async Task<IEnumerable<Import?>> GetAll()
        {
            return await _importRepository.FindAll();
        }

        public async Task<Import?> GetById(Guid id)
        {
            return await _importRepository.FindById(id);
        }

        public async Task UpdateInt(int id, Import entity)
        {
            var exsistingImport = await _importRepository.FindByIdInt(id) ?? throw new ArgumentException("import not exist");
            entity.Id = exsistingImport.Id;
            await _importRepository.Update(entity);
        }

        public Task Update(Guid id, Import entity)
        {
            throw new NotImplementedException();
        }

    }
}
