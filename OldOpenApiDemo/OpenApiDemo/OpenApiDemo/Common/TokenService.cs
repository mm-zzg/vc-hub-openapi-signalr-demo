using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiDemo.Common
{
    public class TokenService
    {

        public OpenApiOptions Options { get; }
        public TokenService(IOptions<OpenApiOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        public async Task<string> GetAccessToken()
        {
            using var httpClient = new HttpClient(new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            });
            httpClient.BaseAddress = new Uri(Options.Url);
            OpenIdConnectMessage request = new OpenIdConnectMessage
            {
                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret,
                GrantType = OpenIdConnectGrantTypes.ClientCredentials
            };
            var config = await httpClient.GetFromJsonAsync<OpenIdConfiguration>("/.well-known/openid-configuration");
            var content = new FormUrlEncodedContent(request.Parameters);
            var response = await httpClient.PostAsync(config!.TokenEndpoint, content);
            var map = await response.Content.ReadFromJsonAsync<IDictionary<string, object>>();
            var tokenResponse = new OpenIdConnectMessage();
            foreach (var (key, value) in map)
            {
                tokenResponse.SetParameter(key, value.ToString());
            }
            return tokenResponse.AccessToken;
        }

    }
}
