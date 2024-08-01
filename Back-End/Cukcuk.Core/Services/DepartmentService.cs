using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;

namespace Cukcuk.Core.Services
{
    public class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository = departmentRepository;

        public async Task Create(Department department)
        {
            department.DepartmentId = Guid.NewGuid();
            department.CreatedDate = DateTime.UtcNow;
            department.ModifiedDate = department.CreatedDate;
            await _departmentRepository.Create(department);
        }

        public async Task DeleteById(Guid id)
        {
            var existingDepartment = await _departmentRepository.FindById(id);
            if (existingDepartment != null)
            {
                await _departmentRepository.DeleteById(id);
            }
            else
            {
                throw new Exception("Department not exists");
            }
        }

        public async Task<IEnumerable<Department?>> GetAll()
        {
            return await _departmentRepository.FindAll();
        }

        public async Task<Department?> GetById(Guid id)
        {
            return await _departmentRepository.FindById(id);
        }

        public async Task Update(Guid id,Department department)
        {
            var existingDepartment = await _departmentRepository.FindById(id);
            if (existingDepartment != null)
            {
                department.DepartmentId = id;
                department.ModifiedDate = DateTime.UtcNow;

                await _departmentRepository.Update(department);
            }
            else
            {
                throw new Exception("Department not exists");
            }
        }
    }
}
