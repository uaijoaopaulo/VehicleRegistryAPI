namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Database
{
    public interface IDbRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> InsertOneAsync(T entity);
        Task<T> UpdateOneAsync(T entity);
        Task DeleteOneAsync(int id);
    }
}