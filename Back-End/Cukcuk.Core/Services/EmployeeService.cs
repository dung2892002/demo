using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Helper;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Cukcuk.Core.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository, IDepartmentService departmentService, 
        IPositionService positionService, IImportRepository importRepository,
        Cache cacheService) : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IDepartmentService _departmentService = departmentService;
        private readonly IPositionService _positionService = positionService;
        private readonly IImportRepository _importRepository = importRepository;
        private readonly Cache _cacheService = cacheService;

        public async Task AddDataImport(Guid cacheId)
        {
            var employees = _cacheService.GetFromCache<EmployeeImportResponse>(cacheId);
            foreach (var employee in employees)
            {
                employee.EmployeeId = Guid.NewGuid();
                employee.CreatedDate = DateTime.UtcNow;
                employee.ModifiedDate = employee.CreatedDate;
                employee.CreatedBy = "admin";
                employee.ModifiedBy = "admin";
                await _employeeRepository.Create(employee);
            }
        }

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

            var newCode = await _employeeRepository.GennerateNewEmployeeCode();
            employeeDTO.EmployeeCode = newCode;
            employeeDTO.EmployeeId = Guid.NewGuid();
            employeeDTO.CreatedDate = DateTime.UtcNow;
            employeeDTO.ModifiedDate = employeeDTO.CreatedDate;
            await _employeeRepository.Create(employeeDTO);
        }

        public byte[] CreateExcelFile(List<EmployeeDTO> employeeDTOs)
        {
            var employees = new List<EmployeeExport>();
            var index = 1;
            foreach (var employee in employeeDTOs)
            {
                employees.Add(new EmployeeExport
                {
                    STT = index++,
                    EmployeeCode = employee.EmployeeCode,
                    Fullname = employee.Fullname,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.GenderName,
                    IdentityNumber = employee.IdentityNumber,
                    IdentityDate = employee.IdentityDate,
                    IdentityPlace = employee.IdentityPlace,
                    Address = employee.Address,
                    MobileNumber = employee.MobileNumber,
                    LandlineNumber = employee.LandlineNumber,
                    Email = employee.Email,
                    BankBranch = employee.BankBranch,
                    BankName = employee.BankName,
                    BankNumber = employee.BankNumber,
                    Position = employee.PositionName,
                    Department = employee.DepartmentName
                });
            }

            var file = ExcelHelper.CreateFile(employees);
            return file;
        }

        public byte[] CreateResultFile(Guid? validCacheId, Guid InvalidCacheId)
        {
            var employees =new List<EmployeeImportResponse>();
            if (validCacheId != null) employees.AddRange(_cacheService.GetFromCache<EmployeeImportResponse>(validCacheId));
            employees.AddRange(_cacheService.GetFromCache<EmployeeImportResponse>(InvalidCacheId));
            
            var employeeReport = new List<EmployeeImportReport>();
            var index = 1;

            foreach (var employee in employees)
            {
                employeeReport.Add(new EmployeeImportReport
                {
                    STT = index++,
                    EmployeeCode = employee.EmployeeCode,
                    Fullname = employee.Fullname,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.GenderName,
                    IdentityNumber = employee.IdentityNumber,
                    IdentityDate = employee.IdentityDate,
                    IdentityPlace = employee.IdentityPlace,
                    Address = employee.Address,
                    MobileNumber = employee.MobileNumber,
                    LandlineNumber = employee.LandlineNumber,
                    Email = employee.Email,
                    BankBranch = employee.BankBranch,
                    BankName = employee.BankName,
                    BankNumber = employee.BankNumber,
                    Position = employee.PositionName,
                    Department = employee.DepartmentName,
                    Status = employee.Status,
                    Errors = employee.Errors,
                });
            }

            var file = ExcelHelper.CreateFile(employeeReport);
            return file;
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

        public async Task<object> ImportExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No files were provided.");

            var imports = await _importRepository.GetByTable("Employee");

            var employees = await ExcelHelper.ReadFile<EmployeeImportResponse>(file, imports);

            var positions = await _positionService.GetAll();
            var departments = await _departmentService.GetAll();

            var listEmployeeCode = new List<string>();
            var listMobileNumber = new List<string>();
            var employeeValid = new List<EmployeeImportResponse>();
            var employeeInvalid = new List<EmployeeImportResponse>();

            foreach (var employee in employees)
            {
                await ExecuteEmployeeData(employee, departments, positions, listEmployeeCode, listMobileNumber, _employeeRepository);

                if (!employee.Status)
                {
                    employeeInvalid.Add(employee);
                } else
                {
                    employeeValid.Add(employee);
                }
            }

            var cacheId = Guid.NewGuid();
            var invalidCacheId = Guid.NewGuid();
            _cacheService.SaveDataToCache<EmployeeImportResponse>(cacheId,employeeValid);
            _cacheService.SaveDataToCache<EmployeeImportResponse>(invalidCacheId, employeeInvalid);
            return new
            {
                TotalValid = employeeValid.Count,
                TotalInvalid = employeeInvalid.Count,
                Datas = employees,
                ValidDataCacheId = cacheId,
                InvalidDataCacheId = invalidCacheId
            };
        }

        private static async Task ExecuteEmployeeData(EmployeeImportResponse employee, 
                                IEnumerable<Department> departments, IEnumerable<Position> positions, 
                                List<string> listEmployeeCode, List<string> listMobileNumber, IEmployeeRepository _employeeRepository)
        {
            employee.Status = true;
            if (employee.DateOfBirth == null)
            {
                employee.Errors.Add("Ngày sinh không hợp lệ");
                employee.Status = false;
            }
            if (employee.IdentityDate == null)
            {
                employee.Errors.Add("Ngày cấp cccd không hợp lệ");
                employee.Status = false;
            }

            var checkCodeSystem = await _employeeRepository.CheckEmployeeCode(employee.EmployeeCode);
            if (!checkCodeSystem)
            {
                employee.Errors.Add("Mã nhân viên đã tồn tại trong hệ thống");
                employee.Status = false;
            }
            var checkCodeTable = listEmployeeCode.Any(code => code == employee.EmployeeCode);
            if (checkCodeTable)
            {
                employee.Errors.Add("Mã nhân viên trùng với nhân viên khác trong tệp nhập khẩu");
                employee.Status = false;
            }
            
            var checkMobileSys = await _employeeRepository.CheckMobileNumber(employee.MobileNumber);
            if (!checkMobileSys)
            {
                employee.Errors.Add("Số điện thoại đã tồn tại trong hệ thống");
                employee.Status = false;
            }
            var checkMobileTable = listMobileNumber.Any(mobile => mobile == employee.MobileNumber);
            if (checkMobileTable)
            {
                employee.Errors.Add("Số điện thoại trùng với số điện thoại của nhân viên khác trong tệp nhập khẩu");
                employee.Status = false;
            }

            //var department = await _departmentService.GetByName(employee.DepartmentName);
            var department = departments.Where(d => d.DepartmentName == employee.DepartmentName).FirstOrDefault();
            if (department == null)
            {
                employee.Errors.Add("Phòng ban không có trong hệ thống");
                employee.Status = false;
            }
            else
            {
                employee.DepartmentId = department.DepartmentId;
            }

            //var position = await _positionService.GetByName(employee.PositionName);
            var position = positions.Where(p => p.PositionName == employee.PositionName).FirstOrDefault();
            if (position == null)
            {
                employee.Errors.Add("Vị trí không có trong hệ thống");
                employee.Status = false;
            }
            else
            {
                employee.PositionId = position.PositionId;
            }

            listEmployeeCode.Add(employee.EmployeeCode);
            listMobileNumber.Add(employee.MobileNumber);
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
