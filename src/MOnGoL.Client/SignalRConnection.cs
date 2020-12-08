using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{
    public class SignalRConnection
    {
        private Task connectTask;

        public HubConnection HubConnection { get; private set; }
        public SignalRConnection(NavigationManager client)
        {
            var uri = client.ToAbsoluteUri("/api/hubs/player");
            HubConnection = new HubConnectionBuilder()
            .WithUrl(uri)
            .Build();
        }

        public async Task Connect()
        {
            if (HubConnection.State == HubConnectionState.Connected)
            {
                return;
            }
            else if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await (connectTask = HubConnection.StartAsync());
            }
            else if (connectTask is not null)
            {
                await connectTask;
            }
        }
    }
}
