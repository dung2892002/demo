using Cukcuk.Core.DTOs;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;

namespace Cukcuk.Core.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository, IDepartmentService departmentService, IPositionService positionService) : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IDepartmentService _departmentService = departmentService;
        private readonly IPositionService _positionService = positionService;
        public async Task Create(EmployeeDTO employeeDTO)
        {
            if (employeeDTO.PositionId == Guid.Empty || employeeDTO.DepartmentId == Guid.Empty)
            {
                throw new ArgumentException("PositionId and DepartmentId are required");
            }

            var existingDepartment = await _departmentService.GetById(employeeDTO.DepartmentId);
            if (existingDepartment == null)
            {
                throw new InvalidOperationException("Department not exists");
            }

            var existingPosition = await _positionService.GetById(employeeDTO.PositionId);
            if (existingPosition == null)
            {
                throw new InvalidOperationException("Position not exists");
            }

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
            if (employeeDTO.PositionId == Guid.Empty || employeeDTO.DepartmentId == Guid.Empty)
            {
                throw new ArgumentException("PositionId and DepartmentId are required");
            }

            var existingDepartment = await _departmentService.GetById(employeeDTO.DepartmentId);
            if (existingDepartment == null)
            {
                throw new InvalidOperationException("Department not exists");
            }

            var existingPosition = await _positionService.GetById(employeeDTO.PositionId);
            if (existingPosition == null)
            {
                throw new InvalidOperationException("Position not exists");
            }

            var existingEmployee = await _employeeRepository.FindById(id);
            if (existingEmployee == null)
            {
                throw new InvalidOperationException("Employee not exists");
            }

            employeeDTO.EmployeeId = id;
            employeeDTO.ModifiedDate = DateTime.UtcNow;
            await _employeeRepository.Update(employeeDTO);
        }
    }
}
