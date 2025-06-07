using VehicleRegistry.Contracts.InfraStructure.Http;
using HttpContent = VehicleRegistry.Contracts.InfraStructure.Http.HttpContent;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Http
{
    public interface IHttpClient
    {
        Task<HttpResponse> GetAsync(string baseUri, string resource, Dictionary<string, string> headers = null, TimeSpan timeOut = default);

        Task<HttpResponse> DeleteAsync(string baseUri, string resource, Dictionary<string, string> headers = null, TimeSpan timeOut = default);

        Task<HttpResponse> PostAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> headers = null, TimeSpan timeOut = default);

        Task<HttpResponse> PutAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> headers = null, TimeSpan timeOut = default);

        Task<HttpResponse> PatchAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> headers = null, TimeSpan timeOut = default);
    }
}