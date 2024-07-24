using BE__Back_End_.Models;
using BE__Back_End_.Repositories.IRepo;
using BE__Back_End_.Services.IService;

namespace BE__Back_End_.Services
{
    public class DepartmentService(IDepartmentRepo departmetnRepo) : IDepartmentService
    {
        private readonly IDepartmentRepo _departmentRepo = departmetnRepo;

        public async Task<IEnumerable<Department?>> GetDepartments()
        {
            return await _departmentRepo.FindAll();
        }

        public async Task<Department?> GetDepartmentById(Guid id)
        {
            return await _departmentRepo.FindById(id);
        }

        public async Task CreateDepartment(Department department)
        {
            department.DepartmentId = Guid.NewGuid();
            department.CreatedDate = DateTime.UtcNow;
            department.ModifiedDate = department.CreatedDate;
            await _departmentRepo.Create(department);
        }

        public async Task UpdateDepartment(Guid id, Department department)
        {
            var existingDepartment = await _departmentRepo.FindById(id);
            if (existingDepartment == null)
            {
                throw new Exception("Department not exists");
            }

            department.DepartmentId = id;
            department.ModifiedDate = DateTime.UtcNow;

            await _departmentRepo.Update(department);
        }

        public async Task DeleteDepartment(Guid id)
        {
            var existingDepartment = await _departmentRepo.FindById(id);
            if (existingDepartment == null)
            {
                throw new Exception("Department not exists");
            }

            await _departmentRepo.DeleteById(id);
        }
    }
}
