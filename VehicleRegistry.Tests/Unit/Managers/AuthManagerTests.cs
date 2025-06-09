using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Manager.User;
using VehicleRegistry.Manager;

namespace VehicleRegistry.Tests.Unit.Managers
{
    public class AuthManagerTests
    {
        private readonly Mock<IUsersRepository> _mockUserRepo;
        private readonly Mock<ILogger<AuthManager>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthManager _authManager;

        public AuthManagerTests()
        {
            _mockUserRepo = new Mock<IUsersRepository>();
            _mockLogger = new Mock<ILogger<AuthManager>>();

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("Th3-super-secret-key-@-extra-safe-123");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authManager = new AuthManager(_mockConfig.Object, _mockLogger.Object, _mockUserRepo.Object);
        }

        [Fact]
        public void GenerateToken_ValidUser_ReturnsJwtToken()
        {
            var user = new UserModel
            {
                Email = "test@example.com",
                Roles = new List<string> { "admin", "user" }
            };

            var token = _authManager.GenerateToken(user);

            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("test@example.com", jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "admin");
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "user");
        }

        [Fact]
        public async Task ValidateUserAsync_ValidCredentials_ReturnsUser()
        {
            var expectedUser = new UserModel
            {
                Email = "test@example.com",
                Password = "pass",
                Roles = new List<string> { "admin" }
            };

            _mockUserRepo.Setup(repo => repo.GetUserByAuthAsync("test@example.com", "pass"))
                         .ReturnsAsync(expectedUser);

            var user = await _authManager.ValidateUserAsync("test@example.com", "pass");

            Assert.NotNull(user);
            Assert.Equal("test@example.com", user!.Email);
            Assert.Contains("admin", user.Roles);
        }

        [Fact]
        public async Task ValidateUserAsync_InvalidCredentials_ReturnsNull()
        {
            _mockUserRepo.Setup(repo => repo.GetUserByAuthAsync("wrong@example.com", "wrong"))
                         .ReturnsAsync((UserModel?)null);

            var user = await _authManager.ValidateUserAsync("wrong@example.com", "wrong");
            Assert.Null(user);
        }
    }
}
