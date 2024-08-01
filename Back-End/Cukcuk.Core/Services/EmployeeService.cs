using Cukcuk.Core.DTOs;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;

namespace Cukcuk.Core.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        public async Task Create(EmployeeDTO employeeDTO)
        {
            employeeDTO.EmployeeId = Guid.NewGuid();
            employeeDTO.CreatedDate = DateTime.UtcNow;
            employeeDTO.ModifiedDate = employeeDTO.CreatedDate;
            await _employeeRepository.Create(employeeDTO);
        }

        public async Task DeleteById(Guid id)
        {
            await _employeeRepository.DeleteById(id);
        }

        public async Task<object> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
        {
            var (employees, totalRecords) = await _employeeRepository.FilterEmployees(pageSize, pageNumber, employeeFilter, departmentId, positionId);
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var response = new
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = employees
            };

            return response;
        }

        public async Task<IEnumerable<EmployeeDTO?>> GetAll()
        {
            return await _employeeRepository.FindAll();
        }

        public async Task<EmployeeDTO?> GetById(Guid id)
        {
            return await _employeeRepository.FindById(id);
        }

        public async Task<string> GetNewEmployeeCode()
        {
            return await _employeeRepository.GennerateNewEmployeeCode();
        }

        public async Task Update(Guid id, EmployeeDTO employeeDTO)
        {
            employeeDTO.EmployeeId = id;
            employeeDTO.ModifiedDate = DateTime.UtcNow;
            await _employeeRepository.Update(employeeDTO);
        }
    }
}
