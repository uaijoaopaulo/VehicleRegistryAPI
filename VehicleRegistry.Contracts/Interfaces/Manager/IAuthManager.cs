using VehicleRegistry.Contracts.Manager.User;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IAuthManager
    {
        /// <summary>
        /// Generates a JWT token or similar authentication token for the specified user.
        /// </summary>
        /// <param name="user">The user model for whom the token will be generated.</param>
        /// <returns>A string representing the generated authentication token.</returns>
        string GenerateToken(UserModel user);

        /// <summary>
        /// Validates the user's credentials asynchronously.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// A <see cref="UserModel"/> representing the authenticated user if validation succeeds; otherwise, <c>null</c>.
        /// </returns>
        Task<UserModel?> ValidateUserAsync(string email, string password);
    }
}
