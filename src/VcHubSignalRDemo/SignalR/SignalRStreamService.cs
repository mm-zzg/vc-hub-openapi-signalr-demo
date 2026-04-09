using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using VcHubSignalRDemo.Auth;
using VcHubSignalRDemo.Models;
using VcHubSignalRDemo.State;

namespace VcHubSignalRDemo.SignalR;

public sealed class SignalRStreamService(
    ITokenService tokenService,
    IVcHubConnectionFactory connectionFactory,
    ConnectionStateStore stateStore,
    ILogger<SignalRStreamService> logger) : ISignalRStreamService, IAsyncDisposable
{
    private HubConnection? _connection;
    private CancellationTokenSource? _tagValuesCts;
    private CancellationTokenSource? _alarmsCts;

    public async Task TestAuthenticationAsync(ConnectionSettings settings, CancellationToken cancellationToken = default)
    {
        await tokenService.GetAccessTokenAsync(settings, cancellationToken);
        stateStore.AddEvent("Info", "Authentication succeeded.");
    }

    public async Task ConnectAsync(ConnectionSettings settings, CancellationToken cancellationToken = default)
    {
        if (_connection is { State: HubConnectionState.Connected })
        {
            return;
        }

        stateStore.SetConnectionStatus(DemoConnectionStatus.Connecting);

        try
        {
            _connection = connectionFactory.Create(settings);
            _connection.Closed += HandleClosedAsync;

            await _connection.StartAsync(cancellationToken);
            stateStore.SetConnectionStatus(DemoConnectionStatus.Connected);
            stateStore.AddEvent("Info", $"Connected to {settings.PrimaryRoute}.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to SignalR route.");
            stateStore.SetConnectionStatus(DemoConnectionStatus.Error, ex.Message);
            stateStore.AddEvent("Error", $"Connection failed: {ex.Message}");
            throw;
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await StopTagValuesAsync();
        await StopAlarmsAsync();

        if (_connection is null)
        {
            stateStore.SetConnectionStatus(DemoConnectionStatus.Disconnected);
            return;
        }

        try
        {
            await _connection.StopAsync(cancellationToken);
            await _connection.DisposeAsync();
            stateStore.AddEvent("Info", "Disconnected from SignalR.");
        }
        finally
        {
            _connection = null;
            stateStore.SetConnectionStatus(DemoConnectionStatus.Disconnected);
        }
    }

    public Task StartTagValuesAsync(IEnumerable<string> tagPaths, CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        var paths = tagPaths.Where(static x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
        if (paths.Length == 0)
        {
            throw new InvalidOperationException("At least one tag path is required for TagValues.");
        }

        _tagValuesCts?.Cancel();
        _tagValuesCts?.Dispose();
        _tagValuesCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        stateStore.SetTagValuesActive(true);
        stateStore.AddEvent("Info", $"TagValues subscription started for {paths.Length} tag(s).");

        _ = Task.Run(async () =>
        {
            try
            {
                var stream = _connection!.StreamAsync<TagValueMessage[]>("TagValues", paths, _tagValuesCts.Token);
                await foreach (var payload in stream.WithCancellation(_tagValuesCts.Token))
                {
                    foreach (var item in payload)
                    {
                        stateStore.AddTagValue(item);
                    }

                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the stream is stopped by the user.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "TagValues stream failed.");
                stateStore.SetConnectionStatus(DemoConnectionStatus.Error, ex.Message);
                stateStore.AddEvent("Error", $"TagValues stream failed: {ex.Message}");
            }
            finally
            {
                stateStore.SetTagValuesActive(false);
            }
        }, _tagValuesCts.Token);

        return Task.CompletedTask;
    }

    public Task StopTagValuesAsync()
    {
        if (_tagValuesCts is null)
        {
            stateStore.SetTagValuesActive(false);
            return Task.CompletedTask;
        }

        _tagValuesCts.Cancel();
        _tagValuesCts.Dispose();
        _tagValuesCts = null;
        stateStore.SetTagValuesActive(false);
        stateStore.AddEvent("Info", "TagValues subscription stopped.");

        return Task.CompletedTask;
    }

    public Task StartAlarmsAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        _alarmsCts?.Cancel();
        _alarmsCts?.Dispose();
        _alarmsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        stateStore.SetAlarmsActive(true);
        stateStore.AddEvent("Info", "Alarms subscription started.");

        _ = Task.Run(async () =>
        {
            try
            {
                var stream = _connection!.StreamAsync<AlarmMessage[]>("Alarms", _alarmsCts.Token);
                await foreach (var payload in stream.WithCancellation(_alarmsCts.Token))
                {
                    foreach(var item in payload)
                    {
                        stateStore.AddAlarm(item);
                    }   
                  
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the stream is stopped by the user.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Alarms stream failed.");
                stateStore.SetConnectionStatus(DemoConnectionStatus.Error, ex.Message);
                stateStore.AddEvent("Error", $"Alarms stream failed: {ex.Message}");
            }
            finally
            {
                stateStore.SetAlarmsActive(false);
            }
        }, _alarmsCts.Token);

        return Task.CompletedTask;
    }

    public Task StopAlarmsAsync()
    {
        if (_alarmsCts is null)
        {
            stateStore.SetAlarmsActive(false);
            return Task.CompletedTask;
        }

        _alarmsCts.Cancel();
        _alarmsCts.Dispose();
        _alarmsCts = null;
        stateStore.SetAlarmsActive(false);
        stateStore.AddEvent("Info", "Alarms subscription stopped.");

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    private Task HandleClosedAsync(Exception? error)
    {
        stateStore.SetTagValuesActive(false);
        stateStore.SetAlarmsActive(false);

        if (error is null)
        {
            stateStore.SetConnectionStatus(DemoConnectionStatus.Disconnected);
            stateStore.AddEvent("Warning", "SignalR connection closed.");
            return Task.CompletedTask;
        }

        stateStore.SetConnectionStatus(DemoConnectionStatus.Error, error.Message);
        stateStore.AddEvent("Error", $"SignalR connection closed unexpectedly: {error.Message}");
        return Task.CompletedTask;
    }

    private void EnsureConnected()
    {
        if (_connection is not { State: HubConnectionState.Connected })
        {
            throw new InvalidOperationException("SignalR connection is not established.");
        }
    }

    private static IEnumerable<TagValueMessage> ParseTagValuePayload(JsonElement payload)
    {
        if (payload.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in payload.EnumerateArray())
            {
                yield return ParseTagValue(item);
            }

            yield break;
        }

        yield return ParseTagValue(payload);
    }

    private static IEnumerable<AlarmMessage> ParseAlarmPayload(JsonElement payload)
    {
        if (payload.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in payload.EnumerateArray())
            {
                yield return ParseAlarm(item);
            }

            yield break;
        }

        yield return ParseAlarm(payload);
    }

    private static TagValueMessage ParseTagValue(JsonElement element)
    {
        return new TagValueMessage
        {
            Path = ReadString(element, "path"),
            Value = ReadRawValue(element, "value"),
            Quality = ReadString(element, "quality"),
            Time = ReadDateTimeOffset(element, "time") ?? ReadDateTimeOffset(element, "timestamp"),
            RawJson = element.GetRawText(),
            ReceivedAt = DateTimeOffset.Now
        };
    }

    private static AlarmMessage ParseAlarm(JsonElement element)
    {
        return new AlarmMessage
        {
            EventId = ReadString(element, "eventId"),
            Path = ReadString(element, "path"),
            Priority = ReadString(element, "priority"),
            Status = ReadString(element, "status"),
            EventTime = ReadDateTimeOffset(element, "eventTime") ?? ReadDateTimeOffset(element, "timestamp"),
            Description = ReadString(element, "description"),
            RawJson = element.GetRawText(),
            ReceivedAt = DateTimeOffset.Now
        };
    }

    private static string ReadString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return string.Empty;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString() ?? string.Empty,
            JsonValueKind.Null => string.Empty,
            _ => property.GetRawText()
        };
    }

    private static string ReadRawValue(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return string.Empty;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString() ?? string.Empty,
            JsonValueKind.Null => string.Empty,
            _ => property.GetRawText()
        };
    }

    private static DateTimeOffset? ReadDateTimeOffset(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(property.GetString(), out var parsed))
        {
            return parsed;
        }

        return null;
    }
}
