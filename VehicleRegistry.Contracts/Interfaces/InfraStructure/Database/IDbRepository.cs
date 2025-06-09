namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Database
{
    public interface IDbRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Retrieves a single entity of type <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Inserts a new entity of type <typeparamref name="T"/> into the database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>The inserted entity with any generated values (e.g., ID).</returns>
        Task<T> InsertOneAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type <typeparamref name="T"/> in the database.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateOneAsync(T entity);

        /// <summary>
        /// Deletes an entity of type <typeparamref name="T"/> from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        Task DeleteOneAsync(int id);
    }
}