using Cukcuk.Core.Entities;
using NPOI.SS.Formula.Functions;

namespace Cukcuk.Core.Interfaces.IServices
{
    public interface IImportService : IBaseService<Import>
    {
        Task UpdateInt(int id, Import entity);
        Task DeleteInt(int id);
    }
}
