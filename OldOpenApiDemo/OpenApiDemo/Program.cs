using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenApiDemo.Common;

namespace OpenApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<OpenApiDemoService>();
            builder.Services.AddHttpClient(HttpClientNames.OpenApi).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                //暂时先将证书验证关闭，不建议生产环境关闭证书验证
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            }).ConfigureAdditionalHttpMessageHandlers((handlers, provider) =>
            {
                handlers.Add(provider.GetRequiredService<AccessTokenHandler>());
            }).ConfigureHttpClient((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<OpenApiOptions>>().Value;
                client.BaseAddress = new Uri(options.Url);
            });

            builder.Services.AddTransient<AccessTokenHandler>();
            builder.Services.AddSingleton<TokenService>();
            builder.Services.AddSingleton<IHubConnectionFactory, HubConnectionFactory>();
            builder.Services.AddMemoryCache();

            builder.Services.Configure<OpenApiOptions>(builder.Configuration.GetSection("OpenApi"));

            var host = builder.Build();
            host.Run();
        }
    }
}