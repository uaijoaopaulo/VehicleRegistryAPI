using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace VehicleRegistry.Contracts.InfraStructure.Http
{
    [ExcludeFromCodeCoverage]
    public abstract class HttpContent
    {
        public abstract Task<System.Net.Http.HttpContent> ReadContentAsync();
    }

    [ExcludeFromCodeCoverage]
    public class JsonContent : HttpContent
    {
        public readonly object Content;

        public JsonContent(object content)
        {
            Content = content;
        }

        public override Task<System.Net.Http.HttpContent> ReadContentAsync() => Task.FromResult<System.Net.Http.HttpContent>(new System.Net.Http.StringContent(JsonConvert.SerializeObject(Content), Encoding.UTF8, "application/json"));
    }

    [ExcludeFromCodeCoverage]
    public class MultipartFormDataContent(string fileName, Stream stream) : HttpContent
    {
        public readonly string FileName = fileName;

        public readonly Stream Stream = stream;

        public readonly object? Parameters;

        public override async Task<System.Net.Http.HttpContent> ReadContentAsync()
        {
            try
            {
                var multipartFormDataContent = new System.Net.Http.MultipartFormDataContent();

                byte[]? bytes = null;

                using (var memoryStream = new MemoryStream())
                {
                    await Stream.CopyToAsync(memoryStream).ConfigureAwait(false);

                    bytes = memoryStream.ToArray();
                }

                multipartFormDataContent.Add(new System.Net.Http.ByteArrayContent(bytes), FileName, FileName);

                if (Parameters != null)
                {
                    foreach (var item in Parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var value = item.GetValue(Parameters);
                        string? stringValue = value != null ? value.ToString() : string.Empty;

                        if (stringValue != null)
                        {
                            multipartFormDataContent.Add(new System.Net.Http.StringContent(item.Name), stringValue);
                        }
                    }
                }
                multipartFormDataContent.Add(new System.Net.Http.StringContent("NORMAL"), "import_mode");

                return multipartFormDataContent;
            }
            finally
            {
                Stream.Dispose();
            }
        }
    }
}
