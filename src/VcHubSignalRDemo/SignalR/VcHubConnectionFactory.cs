using System.Net.Security;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using VcHubSignalRDemo.Auth;
using VcHubSignalRDemo.Configuration;
using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.SignalR;

public sealed class VcHubConnectionFactory(
    ITokenService tokenService,
    IOptions<SignalROptions> signalROptions) : IVcHubConnectionFactory
{
    public HubConnection Create(ConnectionSettings settings)
    {
        var baseUri = settings.Url.EndsWith('/') ? settings.Url : settings.Url + "/";
        var route = settings.PrimaryRoute.TrimStart('/');
        var hubUri = new Uri(new Uri(baseUri), route);

        var bypassCertificateValidation = signalROptions.Value.EnableCertificateValidationBypassForDemo;

        return new HubConnectionBuilder()
            .WithUrl(hubUri, options =>
            {
                options.Transports = HttpTransportType.WebSockets;
                options.AccessTokenProvider = async () => (string?)await tokenService.GetAccessTokenAsync(settings);

                if (bypassCertificateValidation)
                {
                    options.HttpMessageHandlerFactory = _ => new SocketsHttpHandler
                    {
                        SslOptions = new SslClientAuthenticationOptions
                        {
                            RemoteCertificateValidationCallback = (_, _, _, _) => true
                        }
                    };

                    options.WebSocketConfiguration = ws =>
                    {
                        ws.RemoteCertificateValidationCallback = (_, _, _, _) => true;
                    };
                }
            })
            .WithAutomaticReconnect()
            .Build();
    }
}
