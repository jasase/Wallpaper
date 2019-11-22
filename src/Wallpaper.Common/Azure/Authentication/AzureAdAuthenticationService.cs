//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Graph;
//using Newtonsoft.Json;

//namespace Plugin.Application.Wallpaper.Common.Azure.Authentication
//{
//    public class AzureAdAuthenticationService : IAuthenticationProvider
//    {
//        private readonly IRefreshTokenStore _refreshTokenStore;
//        private readonly string _clientId;
//        private readonly string _clientSecret;
//        private readonly HttpClient _httpClient;

//        private string _currentAccessToken;
//        private string _currentAccessTokenType;
//        private string _currentRefreshToken;
//        private DateTime _validUntil;

//        public string RefreshToken => _currentRefreshToken;

//        public AzureAdAuthenticationService(IRefreshTokenStore refreshTokenStore, string clientId, string clientSecret)
//        {
//            _refreshTokenStore = refreshTokenStore;
//            _clientId = clientId;
//            _clientSecret = clientSecret;

//            _httpClient = new HttpClient()
//            {
//                BaseAddress = new Uri("https://login.microsoftonline.com")
//            };
//        }

//        public void InitWithAccessToken(string accessToken)
//        {
//            var uri = "/common/oauth2/v2.0/token?";
//            var query = $"grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer" +
//                        $"&client_id={_clientId}" +
//                        $"&client_secret={_clientSecret}" +
//                        $"&assertion={accessToken}" +
//                        $"&scope=https://graph.microsoft.com/files.readWrite.all+offline_access" +
//                        $"&requested_token_use=on_behalf_of";
//            var result = _httpClient.PostAsync(uri,
//                                              new StringContent(query, Encoding.Default, "application/x-www-form-urlencoded"))
//                                            .Result;

//            result.EnsureSuccessStatusCode();
//            var content = result.Content.ReadAsStringAsync().Result;
//            var contentTyped = JsonConvert.DeserializeObject<AzureAdTokenRequestResult>(content);

//            _currentRefreshToken = contentTyped.refresh_token;
//            _currentAccessToken = contentTyped.access_token;
//            _currentAccessTokenType = contentTyped.token_type;
//            _validUntil = DateTime.Now.AddSeconds(contentTyped.expires_in - 60);

//            _refreshTokenStore.StoreRefreshToken(_currentRefreshToken);
//        }

//        public void InitWithRefreshToken(string refreshToken) => RequestTokens(refreshToken);

//        private void RequestTokens(string refreshToken)
//        {
//            var uri = "/common/oauth2/v2.0/token?";
//            var query = $"grant_type=refresh_token" +
//                        $"&client_id={_clientId}" +
//                        $"&client_secret={_clientSecret}" +
//                        $"&refresh_token={refreshToken}" +
//                        $"&scope=https://graph.microsoft.com/files.readWrite.all+offline_access";
//            var result = _httpClient.PostAsync(uri,
//                                              new StringContent(query, Encoding.Default, "application/x-www-form-urlencoded"))
//                                            .Result;

//            result.EnsureSuccessStatusCode();
//            var content = result.Content.ReadAsStringAsync().Result;
//            var contentTyped = JsonConvert.DeserializeObject<AzureAdTokenRequestResult>(content);

//            var decode = Encoding.UTF8.GetString(Convert.FromBase64String(contentTyped.access_token));

//            _currentRefreshToken = contentTyped.refresh_token;
//            _currentAccessToken = contentTyped.access_token;
//            _currentAccessTokenType = contentTyped.token_type;
//            _validUntil = DateTime.Now.AddSeconds(contentTyped.expires_in - 60);

//            _refreshTokenStore.StoreRefreshToken(_currentRefreshToken);
//        }

//        public Task AuthenticateRequestAsync(HttpRequestMessage request)
//        {
//            CheckAccessTokenValid();

//            var tokenTypeString = string.IsNullOrEmpty(_currentAccessTokenType)
//                    ? "Bearer"
//                    : _currentAccessTokenType;

//            request.Headers.Authorization = new AuthenticationHeaderValue(tokenTypeString, _currentAccessToken);

//            return Task.CompletedTask;
//        }

//        private void CheckAccessTokenValid()
//        {
//            if (DateTime.Now > _validUntil)
//            {
//                RequestTokens(_currentRefreshToken);
//            }
//        }
//    }
//}
