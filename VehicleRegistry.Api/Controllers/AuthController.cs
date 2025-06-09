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
                    _logger.LogInformation($"Falha de autenticação para o usuário '{request.Username}'");
                    return Unauthorized(ApiResponseHelper.Failure("Usuário ou senha inválidos"));
                }

                var token = _authManager.GenerateToken(user);

                _logger.LogInformation($"Usuário '{request.Username}' autenticado com sucesso");

                var response = new LoginResponse
                {
                    Token = token,
                    Roles = user.Roles
                };

                return Ok(ApiResponseHelper.Success(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro inesperado durante autenticação para o usuário '{request.Username}'");

                return BadRequest(ApiResponseHelper.Failure("Erro interno no servidor. Tente novamente mais tarde."));
            }
        }
    }
}
