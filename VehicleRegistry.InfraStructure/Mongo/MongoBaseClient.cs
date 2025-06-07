using MongoDB.Driver;
using VehicleRegistry.Contracts.Interfaces.Mongo;

namespace VehicleRegistry.InfraStructure.Mongo
{
    public abstract class MongoBaseClient<T>(IMongoDatabase mongoDatabase) : IRepositoryMongoBase<T> where T : class
    {
        private readonly IMongoDatabase _mongoDatabase = mongoDatabase;

        /// <summary>
        /// Asynchronously inserts a new document into the MongoDB collection.
        /// </summary>
        /// <param name="model">The document to be inserted.</param>
        /// <returns>A task representing the asynchronous operation, containing the inserted document.</returns>
        public async Task<T> InsertOneAsync(T model)
        {
            await GetCollection<T>().InsertOneAsync(model);
            return model;
        }

        /// <summary>
        /// Asynchronously updates a single document in the MongoDB collection that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter definition used to select the document to update.</param>
        /// <param name="update">The update definition specifying the changes to apply.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        protected async Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            await GetCollection<T>().UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Replaces an existing document in the MongoDB collection that matches the specified filter. If no document is found, a new one is inserted.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="filter">The filter to identify the document to replace.</param>
        /// <param name="model">The new document to insert or update.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated document.</returns>
        protected async Task<T> ReplaceOneAsync(FilterDefinition<T> filter, T model)
        {
            await GetCollection<T>().ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
            return model;
        }

        /// <summary>
        /// Retrieves a single document from the MongoDB collection that matches the specified filter.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="filter">The filter definition to identify the document.</param>
        /// <returns>The matched document, or null if no match is found.</returns>
        protected async Task<T?> GetOneAsync(FilterDefinition<T> filter)
        {
            return (await GetCollection<T>().FindAsync(filter)).SingleOrDefault();
        }

        /// <summary>
        /// Asynchronously retrieves a list of documents from the MongoDB collection based on the specified filter,
        /// with optional sorting, pagination (skip), and limiting of results.
        /// </summary>
        /// <param name="filter">The filter definition to apply to the query.</param>
        /// <param name="sort">An optional sort definition to order the results.</param>
        /// <param name="skip">An optional number of documents to skip (used for pagination).</param>
        /// <param name="limit">An optional limit on the number of documents to return.</param>
        /// <returns>A task representing the asynchronous operation, containing the list of matching documents.</returns>
        protected async Task<List<T>> GetAllAsync(FilterDefinition<T> filter, SortDefinition<T>? sort = null, int? skip = null, int? limit = null)
        {
            var query = GetCollection<T>().Find(filter);

            if (sort is not null)
            {
                query = query.Sort(sort);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Deletes a single document from the MongoDB collection that matches the specified filter.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="filter">The filter to identify the document to delete.</param>
        protected async Task DeleteOneAsync(FilterDefinition<T> filter)
        {
            await GetCollection<T>().DeleteOneAsync(filter);
        }

        /// <summary>
        /// Deletes multiple documents from the MongoDB collection that match the specified filter.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="filter">The filter to identify the documents to delete.</param>
        protected async Task DeleteManyAsync(FilterDefinition<T> filter)
        {
            await GetCollection<T>().DeleteManyAsync(filter);
        }

        /// <summary>
        /// Determines the collection name based on the specified model type.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <returns>The name of the MongoDB collection.</returns>
        protected abstract string GetCollectionName<T>();

        /// <summary>
        /// Retrieves the MongoDB collection for the specified model type, using the default collection name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <returns>The MongoDB collection instance.</returns>
        private IMongoCollection<T> GetCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(GetCollectionName<T>());
        }
    }
}
