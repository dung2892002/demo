namespace Cukcuk.Core.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        Task<IEnumerable<T?>> GetAll();
        Task<T?> GetById(Guid id);
        Task Create(T entity);
        Task Update(Guid id,T entity);
        Task DeleteById(Guid id);
    }
}
