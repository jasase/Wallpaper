using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Framework.Abstraction.Extension;
using Newtonsoft.Json;
using Web.Commands;

namespace Plugin.Application.Wallpaper.Client.Mangers
{
    public class WallpaperApiClient
    {
        private readonly JsonSerializer _serializer;
        private readonly ILogger _logger;
        private readonly AuthenticationManager _authenticationManager;

        public WallpaperApiClient(ILogger logger, AuthenticationManager authenticationManager)
        {
            _serializer = new JsonSerializer();
            _logger = logger;
            _authenticationManager = authenticationManager;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(_authenticationManager.HttpAccessTokenHandler);
            httpClient.BaseAddress = new Uri("https://web.sternheim.eu/api/v1/");
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            //httpClient.BaseAddress = new Uri("http://localhost:5000/api/v1/");

            return httpClient;
        }

        public async Task<Stream> ExecuteStreamCall(string url)
        {
            var client = CreateHttpClient();

            _logger.Debug("Starting request to '{0}'", url);
            var request = client.GetAsync(url);
            if (!request.Wait(TimeSpan.FromSeconds(15)))
            {
                _logger.Warn("Response for request '{0}' timeout", url);
                throw new WebException("Timeout");
            }

            _logger.Debug("Response for request '{0}' received. Status code {1}", url, request.Result.StatusCode);
            var response = request.Result;

            var stream = await response.Content.ReadAsStreamAsync();

            if (response.IsSuccessStatusCode)
                return stream;

            var content = await StreamToStringAsync(stream);
            _logger.Warn("Response for request '{0}' not successfull with content {1}", url, content);
            throw new ApiException((int) response.StatusCode, content);
        }

        public bool PostCommand(Command command)
        {
            var client = CreateHttpClient();

            var serializedObject = JsonConvert.SerializeObject(command, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            var request = client.PostAsync("command", content);
            request.Wait();
            var response = request.Result;

            return response.IsSuccessStatusCode;
        }

        public async Task<TResult> ExecuteApiCall<TResult>(string url)
        {
            var stream = await ExecuteStreamCall(url);
            return DeserializeJsonFromStream<TResult>(stream);
        }

        private async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }

        private T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var searchResult = _serializer.Deserialize<T>(jsonReader);
                return searchResult;
            }
        }
    }
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }

        public ApiException(int statusCode, string content)
            : base($"Exception occurred during request the Wallpaper api with status code {statusCode}." +
                   $"Response content {Environment.NewLine}{content}{Environment.NewLine}")
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
