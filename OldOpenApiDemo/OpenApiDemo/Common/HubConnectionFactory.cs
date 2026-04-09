using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace OpenApiDemo.Common
{
    internal class HubConnectionFactory(TokenService tokenService, IOptions<OpenApiOptions> optionsAccessor) : IHubConnectionFactory
    {


        public HubConnection Create(string route)
        {

            var url = optionsAccessor.Value.Url;
            var uri = new Uri(new Uri(url), route);
            var hubConnection = new HubConnectionBuilder().WithUrl(uri, opt =>
            {
                opt.HttpMessageHandlerFactory = _ => new SocketsHttpHandler
                {
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = (_, _, _, _) => true
                    }
                };
                opt.WebSocketConfiguration = config =>
                {
                    config.RemoteCertificateValidationCallback = (_, _, _, _) => true;
                };
                opt.AccessTokenProvider = async () => await tokenService.GetAccessToken();
                opt.Transports = HttpTransportType.WebSockets;

            }).Build();

            return hubConnection;
        }
    }
}
