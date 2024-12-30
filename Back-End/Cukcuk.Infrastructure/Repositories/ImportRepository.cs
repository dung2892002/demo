using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Dapper;
using jwtAuth.Auth;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cukcuk.Infrastructure.Repositories
{
    public class ImportRepository(IDbConnection connection, ApplicationDbContext dbContext) : IImportRepository
    {
        private readonly IDbConnection _connection = connection;
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(Import entity)
        {
            await _dbContext.Imports.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(Guid id)
        {
            var query = @"delete from import where Id=@Id";
            await _connection.ExecuteAsync(query, new {Id = id});
        }

        public async Task DeleteByIdInt(int id)
        {

            var import = await _dbContext.Imports.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            _dbContext.Imports.Remove(import);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Import?>> FindAll()
        {
            return await _dbContext.Imports.ToListAsync();
        }

        public async Task<Import?> FindByIdInt(int id)
        {
            return await _dbContext.Imports.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Import?> FindById(Guid id)
        {
            var query = "select * from import where Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<Import>(query, new { Id = id });
        }

        public async Task<IEnumerable<Import>> GetByTable(string tableName)
        {
            return await _dbContext.Imports.Where(r => r.TableName==tableName).ToListAsync();
        }


        public async Task Update(Import entity)
        {
           _dbContext.Imports.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
