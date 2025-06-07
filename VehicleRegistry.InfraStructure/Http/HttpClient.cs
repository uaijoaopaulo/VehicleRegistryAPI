using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using VehicleRegistry.Contracts.InfraStructure.Http;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Http;
using HttpContent = VehicleRegistry.Contracts.InfraStructure.Http.HttpContent;

namespace VehicleRegistry.InfraStructure.Http
{
    public class HttpClient : IHttpClient
    {
        private static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        public async Task<HttpResponse> GetAsync(string baseUri, string resource, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan)) =>
            await SendAsync(baseUri, resource, customHeaders: customHeaders, timeOut: timeOut).ConfigureAwait(false);

        public async Task<HttpResponse> DeleteAsync(string baseUri, string resource, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan)) =>
            await SendAsync(baseUri, resource, customHeaders: customHeaders, timeOut: timeOut).ConfigureAwait(false);

        public async Task<HttpResponse> PostAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan)) =>
            await SendAsync(baseUri, resource, content, customHeaders, timeOut).ConfigureAwait(false);

        public async Task<HttpResponse> PutAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan)) =>
            await SendAsync(baseUri, resource, content, customHeaders, timeOut).ConfigureAwait(false);

        public async Task<HttpResponse> PatchAsync(string baseUri, string resource, HttpContent content, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan)) =>
            await SendAsync(baseUri, resource, content, customHeaders, timeOut).ConfigureAwait(false);

        private async Task<HttpResponse> SendAsync(string baseUri, string resource, HttpContent content = null, Dictionary<string, string> customHeaders = null, TimeSpan timeOut = default(TimeSpan), [CallerMemberName] string caller = "")
        {
            if (timeOut == default(TimeSpan))
            {
                timeOut = TimeSpan.FromSeconds(10);
            }

            HttpRequestMessage requestMessage = null;

            switch (caller)
            {
                case nameof(GetAsync):
                    {
                        requestMessage = await BuildRequestMessageAsync(baseUri, resource, HttpMethod.Get, customHeaders).ConfigureAwait(false); break;
                    }
                case nameof(DeleteAsync):
                    {
                        requestMessage = await BuildRequestMessageAsync(baseUri, resource, HttpMethod.Delete, customHeaders).ConfigureAwait(false); break;
                    }
                case nameof(PostAsync):
                    {
                        requestMessage = await BuildRequestMessageAsync(baseUri, resource, HttpMethod.Post, customHeaders, content).ConfigureAwait(false); break;
                    }
                case nameof(PutAsync):
                    {
                        requestMessage = await BuildRequestMessageAsync(baseUri, resource, HttpMethod.Put, customHeaders, content).ConfigureAwait(false); break;
                    }
                case nameof(PatchAsync):
                    {
                        requestMessage = await BuildRequestMessageAsync(baseUri, resource, new HttpMethod("PATCH"), customHeaders, content); break;
                    }
                default:
                    throw new InvalidOperationException();
            }

            try
            {
                using (var cancellationToken = new CancellationTokenSource(timeOut))
                {
                    using (var httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken.Token).ConfigureAwait(false))
                    {
                        return new HttpResponse
                        {
                            Headers = httpResponse.Headers,
                            IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
                            StatusCode = httpResponse.StatusCode,
                            Content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false)
                        };
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return new HttpResponse { Content = "TimeOut" };
            }
        }

        private async Task<HttpRequestMessage> BuildRequestMessageAsync(string baseUri, string resource, HttpMethod method, Dictionary<string, string> customHeaders, HttpContent content = null)
        {
            var uri = new Uri(baseUri);

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{uri.AbsoluteUri}{resource}")
            };

            if (content != null)
            {
                requestMessage.Content = await content.ReadContentAsync();
            }

            if (customHeaders != null && customHeaders.Count > default(int))
            {
                foreach (var item in customHeaders)
                {
                    if (requestMessage.Headers.Contains(item.Key))
                    {
                        continue;
                    }

                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return requestMessage;
        }
    }
}