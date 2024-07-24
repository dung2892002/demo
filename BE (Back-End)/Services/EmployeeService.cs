using BE__Back_End_.Payloads.DTOs;
using BE__Back_End_.Payloads.Request;
using BE__Back_End_.Repositories.IRepo;
using BE__Back_End_.Services.IService;

namespace BE__Back_End_.Services
{
    public class EmployeeService(IEmployeeRepo employeeRepo) : IEmployeeService
    {
        private readonly IEmployeeRepo _employeeRepo = employeeRepo;

        public async Task<IEnumerable<EmployeeResponse?>> GetEmployees()
        {
            return await _employeeRepo.FindAll();
        }

        public async Task<EmployeeResponse?> GetEmployeeById(Guid id)
        {
            return await _employeeRepo.FindById(id);
        }

        public async Task CreateEmployee(EmployeeRequest employeeRequest)
        {
            employeeRequest.EmployeeId = Guid.NewGuid();
            employeeRequest.CreatedDate = DateTime.UtcNow;
            employeeRequest.ModifiedDate = DateTime.UtcNow;
            await _employeeRepo.Create(employeeRequest);
        }

        public async Task UpdateEmployee(Guid id, EmployeeRequest employeeRequest)
        {
            employeeRequest.EmployeeId = id;
            employeeRequest.ModifiedDate = DateTime.UtcNow;
            await _employeeRepo.Update(employeeRequest);
        }

        public async Task DeleteEmployee(Guid id)
        {
            await _employeeRepo.DeleteById(id);
        }

        public async Task<string> GetNewEmployeeCode()
        {
            return await _employeeRepo.GennerateNewEmployeeCode();
        }

        public async Task<object> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
        {
            var (employees, totalRecords) = await _employeeRepo.FilterEmployees(pageSize, pageNumber, employeeFilter, departmentId, positionId);
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

    }
}
