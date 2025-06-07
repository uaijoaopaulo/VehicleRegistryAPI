namespace VehicleRegistry.Contracts.Interfaces.Mongo
{
    public interface IRepositoryMongoBase<T> where T : class
    {
        /// <summary>
        /// Asynchronously inserts a new document into the MongoDB collection.
        /// </summary>
        /// <param name="model">The document to be inserted.</param>
        /// <returns>A task representing the asynchronous operation, containing the inserted document.</returns>
        Task<T> InsertOneAsync(T model);
    }
}