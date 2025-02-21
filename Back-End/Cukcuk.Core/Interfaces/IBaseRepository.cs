namespace Cukcuk.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T?>> FindAll();
        Task<T?> FindById(Guid? id);
        Task Create(T entity);
        Task Update(T entity);
        Task DeleteById(Guid id);
    }
}
