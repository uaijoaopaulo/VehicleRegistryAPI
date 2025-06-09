using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.User;

namespace VehicleRegistry.Manager
{
    public class AuthManager(IConfiguration configuration, ILogger<AuthManager> logger, IUsersRepository userRepository) : IAuthManager
    {
        private readonly ILogger<AuthManager> _logger = logger;
        private readonly IUsersRepository _userRepository = userRepository;
        public string GenerateToken(UserModel user)
        {
            _logger.LogInformation($"Generating token for user: {user.Email}");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email)
            };

            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            _logger.LogDebug($"Token generated successfully for user: {user.Email}");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserModel?> ValidateUserAsync(string email, string password)
        {
            _logger.LogInformation($"Validating user with email: {email}");

            var user = await _userRepository.GetUserByAuthAsync(email, password);

            if (user == null)
            {
                _logger.LogWarning($"Invalid login attempt for email: {email}");
            }
            else
            {
                _logger.LogDebug($"User validated successfully. Email: {user.Email}");
            }

            return user;
        }
    }
}
