using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.SignalR;

public interface ISignalRStreamService
{
    Task TestAuthenticationAsync(ConnectionSettings settings, CancellationToken cancellationToken = default);

    Task ConnectAsync(ConnectionSettings settings, CancellationToken cancellationToken = default);

    Task DisconnectAsync(CancellationToken cancellationToken = default);

    Task StartTagValuesAsync(IEnumerable<string> tagPaths, CancellationToken cancellationToken = default);

    Task StopTagValuesAsync();

    Task StartAlarmsAsync(CancellationToken cancellationToken = default);

    Task StopAlarmsAsync();
}
