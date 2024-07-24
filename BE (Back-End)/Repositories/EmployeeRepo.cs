using BE__Back_End_.Models;
using BE__Back_End_.Payloads.DTOs;
using BE__Back_End_.Payloads.Request;
using BE__Back_End_.Repositories.IRepo;
using Dapper;
using System.Data;

namespace BE__Back_End_.Repositories
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly IDbConnection _connection;

        public EmployeeRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<EmployeeResponse>> FindAll()
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
            return await _connection.QueryAsync<EmployeeResponse>(query);
        }

        public async Task<EmployeeResponse?> FindById(Guid id)
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
            return await _connection.QuerySingleOrDefaultAsync<EmployeeResponse>(query, new { Id = id });
        }

        public async Task Create(EmployeeRequest employeeRequest)
        {
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
                )
            ";
            await _connection.ExecuteAsync(query, employeeRequest);
        }

        public async Task Update(EmployeeRequest employeeRequest)
        {
            var query = @"
                UPDATE employee SET
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
        }

        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM employee WHERE EmployeeId = @Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<string> GennerateNewEmployeeCode()
        {
            var query = @"
                SELECT EmployeeCode
                FROM employee
                ORDER BY CAST(SUBSTRING(EmployeeCode, 4) AS UNSIGNED) DESC
                LIMIT 1;
            ";

            var lastEmployeeCode = await _connection.QueryFirstOrDefaultAsync<string>(query);
            if (lastEmployeeCode == null)
            {
                return "NV-0001";
            }

            int lastNumber = int.Parse(lastEmployeeCode.Substring(3));
            int newNumber = lastNumber + 1;
            return "NV-" + newNumber.ToString("D4");
        }

        public async Task<(IEnumerable<EmployeeResponse> Employees, int TotalCount)> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
        {

            int offset = (pageNumber - 1) * pageSize;
            var query = @"
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

            return (employees, totalRecords);
        }
    }
}
