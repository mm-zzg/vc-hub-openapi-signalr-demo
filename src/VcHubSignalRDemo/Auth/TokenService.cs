using System.Net.Security;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VcHubSignalRDemo.Configuration;
using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.Auth;

public sealed class TokenService(
    IMemoryCache memoryCache,
    IOptions<SignalROptions> signalROptions,
    ILogger<TokenService> logger) : ITokenService
{
    private sealed class OpenIdMetadata
    {
        public string Token_Endpoint { get; set; } = string.Empty;
    }

    public async Task<string> GetAccessTokenAsync(ConnectionSettings settings, CancellationToken cancellationToken = default)
    {
        ValidateSettings(settings);

        var cacheKey = $"access_token::{settings.Url}::{settings.ClientId}::{settings.ClientSecret}";
        if (memoryCache.TryGetValue<string>(cacheKey, out var cached) && !string.IsNullOrWhiteSpace(cached))
        {
            return cached;
        }

        using var httpClient = new HttpClient(CreateHandler(signalROptions.Value.EnableCertificateValidationBypassForDemo))
        {
            BaseAddress = new Uri(settings.Url)
        };

        var metadata = await httpClient.GetFromJsonAsync<OpenIdMetadata>("/.well-known/openid-configuration", cancellationToken);
        if (metadata is null || string.IsNullOrWhiteSpace(metadata.Token_Endpoint))
        {
            throw new InvalidOperationException("OpenID discovery document is missing token_endpoint.");
        }

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = settings.ClientId,
            ["client_secret"] = settings.ClientSecret,
            ["grant_type"] = "client_credentials"
        });

        using var response = await httpClient.PostAsync(metadata.Token_Endpoint, content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Token endpoint call failed with status code {StatusCode}: {Payload}", response.StatusCode, payload);
            throw new InvalidOperationException($"Token request failed with status code {(int)response.StatusCode}.");
        }

        using var document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("access_token", out var accessTokenElement))
        {
            throw new InvalidOperationException("Token response does not contain access_token.");
        }

        var token = accessTokenElement.GetString();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token response returned an empty access token.");
        }

        var cacheDuration = TimeSpan.FromMinutes(10);
        if (document.RootElement.TryGetProperty("expires_in", out var expiresInElement)
            && expiresInElement.TryGetInt32(out var expiresInSeconds)
            && expiresInSeconds > 120)
        {
            cacheDuration = TimeSpan.FromSeconds(expiresInSeconds - 60);
        }

        memoryCache.Set(cacheKey, token, cacheDuration);
        return token;
    }

    private static void ValidateSettings(ConnectionSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Url))
        {
            throw new InvalidOperationException("OpenApi Url is required.");
        }

        if (string.IsNullOrWhiteSpace(settings.ClientId))
        {
            throw new InvalidOperationException("OpenApi ClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(settings.ClientSecret))
        {
            throw new InvalidOperationException("OpenApi ClientSecret is required.");
        }
    }

    private static SocketsHttpHandler CreateHandler(bool bypassCertificateValidation)
    {
        if (!bypassCertificateValidation)
        {
            return new SocketsHttpHandler();
        }

        return new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            }
        };
    }
}
