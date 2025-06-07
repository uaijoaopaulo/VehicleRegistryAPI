namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
    }
}
