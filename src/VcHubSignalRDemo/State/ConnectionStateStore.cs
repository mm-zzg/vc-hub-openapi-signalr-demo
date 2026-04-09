using Microsoft.Extensions.Options;
using VcHubSignalRDemo.Configuration;
using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.State;

public sealed class ConnectionStateStore
{
    private readonly int _maxMessagesPerStream;

    public ConnectionStateStore(IOptions<DemoOptions> demoOptions)
    {
        _maxMessagesPerStream = Math.Max(10, demoOptions.Value.MaxMessagesPerStream);
    }

    public DemoConnectionStatus ConnectionStatus { get; private set; } = DemoConnectionStatus.Disconnected;

    public bool IsTagValuesActive { get; private set; }

    public bool IsAlarmsActive { get; private set; }

    public string? LatestError { get; private set; }

    public List<TagValueMessage> TagValues { get; } = [];

    public List<AlarmMessage> Alarms { get; } = [];

    public List<UiEventLogItem> EventLogs { get; } = [];

    public event Action? Changed;

    public void SetConnectionStatus(DemoConnectionStatus status, string? error = null)
    {
        ConnectionStatus = status;
        LatestError = error;
        Notify();
    }

    public void ClearLatestError()
    {
        LatestError = null;
        Notify();
    }

    public void SetTagValuesActive(bool active)
    {
        IsTagValuesActive = active;
        Notify();
    }

    public void SetAlarmsActive(bool active)
    {
        IsAlarmsActive = active;
        Notify();
    }

    public void AddTagValue(TagValueMessage message)
    {
        TagValues.Insert(0, message);
        Trim(TagValues);
        Notify();
    }

    public void AddAlarm(AlarmMessage message)
    {
        Alarms.Insert(0, message);
        Trim(Alarms);
        Notify();
    }

    public void AddEvent(string level, string message)
    {
        EventLogs.Insert(0, new UiEventLogItem
        {
            Level = level,
            Message = message,
            Time = DateTimeOffset.Now
        });

        Trim(EventLogs);
        Notify();
    }

    public void ClearTagValues()
    {
        TagValues.Clear();
        Notify();
    }

    public void ClearAlarms()
    {
        Alarms.Clear();
        Notify();
    }

    private void Trim<T>(List<T> list)
    {
        if (list.Count <= _maxMessagesPerStream)
        {
            return;
        }

        list.RemoveRange(_maxMessagesPerStream, list.Count - _maxMessagesPerStream);
    }

    private void Notify()
    {
        Changed?.Invoke();
    }
}
