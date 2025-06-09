using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Login;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Manager.User;
using VehicleRegistry.Api.Controllers;

namespace VehicleRegistry.Tests.Unit.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<ILogger<AuthController>> _mockLogger = new();
        private readonly Mock<IAuthManager> _mockAuthManager = new();
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _controller = new AuthController(_mockLogger.Object, _mockAuthManager.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokenAndRoles()
        {
            var request = new LoginRequest { Username = "valid@user.com", Password = "secure123" };
            var user = new UserModel
            {
                Email = "valid@user.com",
                Password = "secure123",
                Roles = new List<string> { "Admin", "User" }
            };
            var token = "valid.jwt.token";

            _mockAuthManager.Setup(m => m.ValidateUserAsync(request.Username, request.Password))
                .ReturnsAsync(user);
            _mockAuthManager.Setup(m => m.GenerateToken(user)).Returns(token);

            var result = await _controller.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(okResult.Value);
            Assert.Equal(token, response.Result?.Token);
            Assert.Equal(user.Roles, response.Result?.Roles);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequest { Username = "invalid@user.com", Password = "wrong" };

            _mockAuthManager.Setup(m => m.ValidateUserAsync(request.Username, request.Password))
                .ReturnsAsync((UserModel?)null);

            var result = await _controller.Login(request);
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(unauthorized.Value);
            Assert.Null(response.Result);
            Assert.Contains("Invalid username or password", response.Errors);
        }

        [Fact]
        public async Task Login_ExceptionThrown_ReturnsBadRequest()
        {
            var request = new LoginRequest { Username = "error@user.com", Password = "error" };

            _mockAuthManager.Setup(m => m.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database connection failure"));

            var result = await _controller.Login(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Null(response.Result);
            Assert.Contains("An unexpected error occurred", response.Errors[0]);
        }
    }
}
