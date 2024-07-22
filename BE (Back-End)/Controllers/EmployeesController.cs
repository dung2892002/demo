using BE__Back_End_.Models;
using BE__Back_End_.Payloads.DTOs;
using BE__Back_End_.Payloads.Request;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BE__Back_End_.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class EmployeesController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public EmployeesController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var query = @"
                        SELECT 
                            e.*, 
                            d.DepartmentId, d.DepartmentName, d.DepartmentCode, 
                            p.PositionId, p.PositionName, p.PositionCode  
                        FROM 
                            employee e
                            LEFT JOIN department d ON e.DepartmentId = d.DepartmentId
                            LEFT JOIN position p ON e.PositionId = p.PositionId
                        ";

                var employees = await _connection.QueryAsync<EmployeeResponse>(query);

                return StatusCode(200, employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            try
            {
                var query = @"
                        SELECT 
                            e.*, 
                            d.DepartmentId, d.DepartmentName, d.DepartmentCode, 
                            p.PositionId, p.PositionName, p.PositionCode  
                        FROM 
                            employee e
                            LEFT JOIN department d ON e.DepartmentId = d.DepartmentId
                            LEFT JOIN position p ON e.PositionId = p.PositionId
                        WHERE e.EmployeeId = @Id
                        ";
                var employee = await _connection.QuerySingleOrDefaultAsync<EmployeeResponse>(query, new { Id = id });

                if (employee == null)
                {
                    return StatusCode(404, "Employee not exist");
                }

                return StatusCode(200, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            try
            {
                if (employeeRequest.PositionId == Guid.Empty || employeeRequest.DepartmentId == Guid.Empty)
                {
                    return StatusCode(400, "PositionId and DepartmentId are required");
                }

                var existingDepartment = await _connection.QuerySingleOrDefaultAsync<Department>(
                  "SELECT * FROM department where DepartmentId = @Id", new { Id = employeeRequest.DepartmentId });
                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                var existingPosition = await _connection.QuerySingleOrDefaultAsync<Position>(
                    "SELECT * FROM position where PositionId = @Id", new { Id = employeeRequest.PositionId });
                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }

                employeeRequest.EmployeeId = Guid.NewGuid();
                employeeRequest.CreatedDate = DateTime.Now;
                employeeRequest.ModifiedDate = DateTime.Now;

                var query = @"
                                INSERT INTO employee (
                                    EmployeeId, EmployeeCode, Fullname, DateOfBirth, Gender, IdentityNumber,
                                    IdentityDate, IdentityPlace, Address, MobileNumber, LandlineNumber, Email,
                                    BankNumber, BankName, BankBranch, DepartmentId, PositionId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                                )
                                VALUES (
                                    @EmployeeId, @EmployeeCode, @Fullname, @DateOfBirth, @Gender, @IdentityNumber,
                                    @IdentityDate, @IdentityPlace, @Address, @MobileNumber, @LandlineNumber, @Email,
                                    @BankNumber, @BankName, @BankBranch, @DepartmentId, @PositionId, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy
                                )";

                await _connection.ExecuteAsync(query, employeeRequest);

                return StatusCode(201, "Created employee succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeRequest employeeRequest)
        {
            try
            {
                if (employeeRequest.PositionId == Guid.Empty || employeeRequest.DepartmentId == Guid.Empty)
                {
                    return StatusCode(400, "PositionId and DepartmentId are required");
                }

                var existingDepartment = await _connection.QuerySingleOrDefaultAsync<Department>(
                  "SELECT * FROM department where DepartmentId = @Id", new { Id = employeeRequest.DepartmentId });
                if (existingDepartment == null)
                {
                    return StatusCode(404, "Department not exists");
                }

                var existingPosition = await _connection.QuerySingleOrDefaultAsync<Position>(
                    "SELECT * FROM position where PositionId = @Id", new { Id = employeeRequest.PositionId });
                if (existingPosition == null)
                {
                    return StatusCode(404, "Position not exists");
                }

                var existingEmployee = await _connection.QuerySingleOrDefaultAsync<Employee>(
                    "SELECT * FROM employee where EmployeeId = @Id", new { Id = id });
                if (existingEmployee == null)
                {
                    return StatusCode(404, "Employee not exists");
                }

                employeeRequest.EmployeeId = id;
                employeeRequest.ModifiedDate = DateTime.Now;

                var query = @"UPDATE employee SET
                                Fullname = @Fullname,
                                DateOfBirth = @DateOfBirth,
                                Gender = @Gender,
                                IdentityNumber = @IdentityNumber,
                                IdentityDate = @IdentityDate,
                                IdentityPlace = @IdentityPlace,
                                Address = @Address,
                                MobileNumber = @MobileNumber,
                                LandlineNumber = @LandlineNumber,
                                Email = @Email,
                                BankNumber = @BankNumber,
                                BankName = @BankName,
                                BankBranch = @BankBranch,
                                DepartmentId = @DepartmentId,
                                PositionId = @PositionId,
                                ModifiedDate = @ModifiedDate,
                                ModifiedBy = @ModifiedBy
                              WHERE EmployeeId = @EmployeeId";

                await _connection.ExecuteAsync(query, employeeRequest);

                return StatusCode(200, "Update employee successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var existingEmployee = await _connection.QuerySingleOrDefaultAsync<Employee>(
                    "SELECT * FROM employee WHERE EmployeeId = @Id", new { Id = id });
                if (existingEmployee == null)
                {
                    return StatusCode(404, "Employee not exists");
                }

                var query = "DELETE FROM employee WHERE EmployeeId = @Id";

                await _connection.ExecuteAsync(query, new { Id = id });

                return StatusCode(200, "Delete employee successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("NewEmployeeCode")]
        public async Task<IActionResult> GetNewEmployeeCode()
        {
            try
            {
                var query = @"
                    SELECT EmployeeCode
                    FROM employee
                    ORDER BY CAST(SUBSTRING(EmployeeCode, 4) AS UNSIGNED) DESC
                    LIMIT 1;";

                var lastEmployeeCode = await _connection.QueryFirstOrDefaultAsync<string>(query);
                if (lastEmployeeCode == null)
                {
                    return StatusCode(200, "NV-0001");
                }
                int lastNumber = int.Parse(lastEmployeeCode.Substring(3));
                int newNumber = lastNumber + 1;
                string newEmployeeCode = "NV-" + newNumber.ToString("D4");
                return StatusCode(200, newEmployeeCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterEmployee(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
        {
            try
            {
                int offset = (pageNumber - 1) * pageSize;

                string query= @"
                    SELECT SQL_CALC_FOUND_ROWS
                        e.*, 
                        d.DepartmentId, d.DepartmentName, d.DepartmentCode, 
                        p.PositionId, p.PositionName, p.PositionCode  
                    FROM 
                        employee e
                        LEFT JOIN department d ON e.DepartmentId = d.DepartmentId
                        LEFT JOIN position p ON e.PositionId = p.PositionId
                    WHERE 1 = 1";

                if (!string.IsNullOrEmpty(employeeFilter))
                {
                    query += " AND (e.EmployeeCode LIKE CONCAT('%', @EmployeeFilter, '%') " +
                             "OR e.Fullname LIKE CONCAT('%', @EmployeeFilter, '%'))";
                }


                if (departmentId != null)
                {
                    query += " AND e.DepartmentId = @departmentId";
                }
                if (positionId != null)
                {
                    query += " AND e.PositionId = @positionId";
                }

                query += " ORDER BY e.EmployeeCode DESC LIMIT @PageSize OFFSET @Offset;";
                query += " SELECT FOUND_ROWS() AS TotalCount;";

                var parameters = new
                {
                    PageSize = pageSize,
                    Offset = offset,
                    EmployeeFilter = employeeFilter,
                    DepartmentId = departmentId,
                    PositionId = positionId
                };

                var multi = await _connection.QueryMultipleAsync(query, parameters);
                var employees = await multi.ReadAsync<EmployeeResponse>();
                var totalRecords = await multi.ReadSingleAsync<int>();

                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = employees
                };

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
