using Microsoft.AspNetCore.SignalR.Client;
using VcHubSignalRDemo.Models;

namespace VcHubSignalRDemo.SignalR;

public interface IVcHubConnectionFactory
{
    HubConnection Create(ConnectionSettings settings);
}
