using Microsoft.AspNetCore.Mvc;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Login;
using VehicleRegistry.Contracts.Interfaces.Manager;

namespace VehicleRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthManager authManager) : ControllerBase
    {
        private readonly IAuthManager _authManager = authManager;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authManager.ValidateUserAsync(request.Username, request.Password);

                if (user is null)
                {
                    return Unauthorized(new ApiResponse<LoginResponse>
                    {
                        Errors = new List<string> { "Usuário ou senha inválidos" }
                    });
                }

                var token = _authManager.GenerateToken(user);
                return Ok(new ApiResponse<LoginResponse>
                {
                    Result = new LoginResponse
                    {
                        Token = token,
                        Roles = user.Roles
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Errors = new List<string> { e.Message }
                });
            }
        }
    }
}
