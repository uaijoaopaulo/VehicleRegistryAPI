using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;

namespace VehicleRegistry.Contracts.InfraStructure.Http
{
    [ExcludeFromCodeCoverage]
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public string? Content { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public HttpResponseHeaders? Headers { get; set; }

        public T? ReadContent<T>() where T : class => string.IsNullOrWhiteSpace(Content) ? null : JsonConvert.DeserializeObject<T>(Content);
    }
}