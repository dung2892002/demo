using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IImportRepository : IBaseRepository<Import>
    {
        Task<IEnumerable<Import>> GetByTable(string tableName);
        Task<Import?> FindByIdInt(int id);

        Task DeleteByIdInt(int id);
    }
}
