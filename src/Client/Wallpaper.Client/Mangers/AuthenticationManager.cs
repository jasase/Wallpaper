using System;
using System.Net.Http;
using System.Threading.Tasks;
using Framework.Abstraction.Extension;
using IdentityModel.OidcClient;

namespace Plugin.Application.Wallpaper.Client.Mangers
{
    public class AuthenticationManager
    {
        private LoginResult _result;
        private readonly OidcClient _client;

        public bool IsAuthenticated
            => _result != null &&
               !_result.IsError &&
               _result.AccessToken != null;

        public HttpMessageHandler HttpAccessTokenHandler
        {
            get
            {
                if (!IsAuthenticated) throw new InvalidOperationException();
                return _result.RefreshTokenHandler;
            }
        }

        public string Username
        {
            get
            {
                if (!IsAuthenticated) return string.Empty;
                var name = _result.User.FindFirst("preferred_username");
                return name != null ? name.Value : "N/A";
            }
        }

        public AuthenticationManager(ILogManager logManager)
        {
            var options = new OidcClientOptions
            {
                Authority = "https://auth.sternheim.eu/auth/realms/sternheim/",
                ClientId = "WallpaperClient",
                Scope = "openid", //offline_access
                RedirectUri = "io.identityserver.demo.uwp://callback",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Browser = new WpfEmbeddedBrowser()
            };

            options.Policy.RequireAccessTokenHash = false;
            options.LoggerFactory.AddProvider(logManager);

            _client = new OidcClient(options);
        }

        public async Task<bool> Login()
        {
            _result = await _client.LoginAsync(new LoginRequest());
            return IsAuthenticated;
        }

        public async Task Logout()
        {
            await _client.LogoutAsync(new LogoutRequest());
            _result = null;
        }

        //public void Test()
        //{
        //    _result.
        //}
    }
}
