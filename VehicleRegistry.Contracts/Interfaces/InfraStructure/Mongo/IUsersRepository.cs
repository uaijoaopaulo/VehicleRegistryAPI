using VehicleRegistry.Contracts.Manager.User;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo
{
    public interface IUsersRepository : IRepositoryMongoBase<UserModel>
    {
        /// <summary>
        /// Retrieves a user based on the provided credentials (email and password).
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// A <see cref="UserModel"/> representing the authenticated user, or <c>null</c> if the credentials are invalid.
        /// </returns>
        Task<UserModel?> GetUserByAuthAsync(string email, string password);
    }
}