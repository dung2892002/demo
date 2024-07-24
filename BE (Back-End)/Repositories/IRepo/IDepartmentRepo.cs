using BE__Back_End_.Models;

namespace BE__Back_End_.Repositories.IRepo
{
    public interface IDepartmentRepo
    {
        Task<IEnumerable<Department>> FindAll();

        Task<Department> FindById(Guid id);

        Task Create(Department department);

        Task Update(Department department);

        Task DeleteById(Guid id);
    }
}
