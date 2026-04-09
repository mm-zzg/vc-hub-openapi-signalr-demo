using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiDemo.Common
{
    public interface IHubConnectionFactory
    {

        HubConnection Create(string route);

    }
}
