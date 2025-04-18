﻿using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.Repositories;
using Cukcuk.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cukcuk.Infrastructure.Repositories
{
    public class EmployeeRepository(IDbConnection connection, ApplicationDbContext dbContext) : IEmployeeRepository
    {
        private readonly IDbConnection _connection = connection;
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<bool> CheckEmployeeCode(string employeeCode)
        {
            var query = @"
                SELECT
                    EmployeeCode
                FROM
                    employee e
                WHERE
                    EmployeeCode = @employeeCode
            ";

            var code = await _connection.QueryFirstOrDefaultAsync<string>(query, new { employeeCode });
            return code == null;
        }

        public async Task<bool> CheckMobileNumber(string mobileNumber)
        {
            var query = @"
                SELECT
                    MobileNumber
                FROM
                    employee e
                WHERE
                    MobileNumber = @mobileNumber
            ";

            var mobile = await _connection.QueryFirstOrDefaultAsync<string>(query, new { mobileNumber });
            return mobile == null;
        }

        public async Task Create(EmployeeDTO employeeDTO)
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
            await _connection.ExecuteAsync(query, employeeDTO);
        }


        public async Task DeleteById(Guid id)
        {
            var query = "DELETE FROM employee WHERE EmployeeId = @Id";
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<(IEnumerable<EmployeeDTO> Employees, int TotalCount)> FilterEmployees(int pageSize, int pageNumber, string? employeeFilter, Guid? departmentId, Guid? positionId)
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
            var employees = await multi.ReadAsync<EmployeeDTO>();
            var totalRecords = await multi.ReadSingleAsync<int>();

            return (employees, totalRecords);
        }

        public async Task<IEnumerable<EmployeeDTO?>> FindAll()
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
            return await _connection.QueryAsync<EmployeeDTO>(query);
        }

        public async Task<EmployeeDTO?> FindById(Guid? id)
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
            return await _connection.QuerySingleOrDefaultAsync<EmployeeDTO>(query, new { Id = id });
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
                return "NV-00001";
            }

            int lastNumber = int.Parse(lastEmployeeCode[3..]);
            int newNumber = lastNumber + 1;
            return "NV-" + newNumber.ToString("D5");
        }


        public async Task CreateFolder(EmployeeFolder folder)
        {
            await _dbContext.AddAsync(folder);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<PageResult<EmployeeFolder>> GetFolder(Guid? parentId, string? keyword, int pageSize, int pageNumber, bool? sortName, bool? sortDate, bool? sortType)
        {
            var query = _dbContext.EmployeesFolders.AsNoTracking().AsQueryable();

            query = query.Where(cf => parentId.HasValue ? cf.ParentId == parentId.Value : cf.ParentId == null);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(cf => cf.Name.Contains(keyword));
            }

            var totalItems = await query.CountAsync();

            if (sortName.HasValue)
            {
                query = sortName.Value ? query.OrderBy(cf => cf.Name) : query.OrderByDescending(cf => cf.Name);
            }

            if (sortDate.HasValue)
            {
                query = sortDate.Value ? query.OrderBy(cf => cf.CreatedAt) : query.OrderByDescending(cf => cf.CreatedAt);
            }

            if (sortType.HasValue)
            {
                query = sortType.Value ? query.OrderBy(cf => cf.Type) : query.OrderByDescending(cf => cf.Type);
            }

            var employeeFolders = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Include(c => c.Employee)
                .ToListAsync();

            return new PageResult<EmployeeFolder>
            {
                Items = employeeFolders,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)

            };
        }

        public async Task Update(EmployeeDTO employeeDTO)
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
            await _connection.ExecuteAsync(query, employeeDTO);
        }

        public async Task<IEnumerable<EmployeeFolder>> GetFolderOnly()
        {
            return await _dbContext.EmployeesFolders.Where(cf => cf.Type == true).ToListAsync();
        }
    }
}
