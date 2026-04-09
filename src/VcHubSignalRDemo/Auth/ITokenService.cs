using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.Auth;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(ConnectionSettings settings, CancellationToken cancellationToken = default);
}
