using Microsoft.EntityFrameworkCore;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;

namespace VehicleRegistry.InfraStructure.Database.Repository
{
    public class DbRepository<T>(AppDbContext context) : IDbRepository<T> where T : class
    {
        protected readonly AppDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> InsertOneAsync(T model)
        {
            _dbSet.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<T> UpdateOneAsync(T model)
        {
            _dbSet.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteOneAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
