using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenApiDemo.Common
{
    public static class TokenHelper
    {


        public static async Task<string> GetAccessToken()
        {
            using var httpClient = new HttpClient(new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            });
            httpClient.BaseAddress = new Uri(Constants.BaseUrl);
            OpenIdConnectMessage request = new OpenIdConnectMessage
            {
                ClientId = "OpenApi",
                ClientSecret = "OpenApi",
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
