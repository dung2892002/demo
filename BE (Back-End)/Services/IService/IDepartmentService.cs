using BE__Back_End_.Models;

namespace BE__Back_End_.Services.IService
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department?>> GetDepartments();
        Task<Department?> GetDepartmentById(Guid id);
        Task CreateDepartment(Department department);
        Task UpdateDepartment(Guid id, Department department);
        Task DeleteDepartment(Guid id);
    }
}
