using Microsoft.AspNetCore.Mvc;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Login;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ILogger<AuthController> logger, IAuthManager authManager) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IAuthManager _authManager = authManager;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authManager.ValidateUserAsync(request.Username, request.Password);

                if (user is null)
                {
                    _logger.LogInformation($"Authentication failed for user '{request.Username}'");
                    return Unauthorized(ApiResponseHelper.Failure("Invalid username or password"));
                }

                var token = _authManager.GenerateToken(user);

                _logger.LogInformation($"User '{request.Username}' successfully authenticated");

                var response = new LoginResponse
                {
                    Token = token,
                    Roles = user.Roles
                };

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error during authentication for user '{request.Username}'");
                return BadRequest(ApiResponseHelper.Failure("An unexpected error occurred. Please try again later."));
            }
        }
    }
}
