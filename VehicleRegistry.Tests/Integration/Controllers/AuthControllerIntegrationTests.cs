using Newtonsoft.Json;
using System.Net;
using System.Text;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Login;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace VehicleRegistry.Tests.Integration.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var loginRequest = new
            {
                username = "analista@acme.com",
                password = "analista123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(json);
            result.Should().NotBeNull();
            result!.Errors.Should().BeEmpty();
            result.Result.Should().NotBeNull();
            result.Result!.Token.Should().NotBeNullOrWhiteSpace();
            result.Result.Roles.Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var loginRequest = new
            {
                username = "admin",
                password = "wrongpassword"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            result.Should().NotBeNull();
            result!.Result.Should().BeNull();
            result.Errors.Should().ContainSingle("Invalid username or password");
        }
    }
}
