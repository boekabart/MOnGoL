using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MyProject.Common;
using System;
using System.Threading.Tasks;

namespace MyProject.Backend.Client
{
    public class CounterServiceWebClient : ICounterService
    {
        private HubConnection hubConnection;
        private EventHandler<int> onNewValue;
        private int? lastValue;

        public EventHandler<int> OnNewValue
        {
            get => onNewValue;
            set
            {
                onNewValue = value;
                _ = Connect();
                if (lastValue.HasValue)
                    OnNewValue?.Invoke(this, lastValue.Value);
            }
        }

        public CounterServiceWebClient(NavigationManager client)
        {
            var uri = client.ToAbsoluteUri("/api/hubs/counter");
            hubConnection = new HubConnectionBuilder()
            .WithUrl(uri)
            .Build();

            hubConnection.On<int>("NewValue", newValue =>
            {
                lastValue = newValue;
                OnNewValue?.Invoke(this, newValue);
            });
        }

        public async Task Increment(int byHowMuch)
        {
            await Connect();
            await hubConnection.SendAsync("Increment", byHowMuch);
        }

        private async Task Connect()
        {
            if (hubConnection.State != HubConnectionState.Connected)
            {
                await hubConnection.StartAsync();
                await hubConnection.SendAsync("Increment", 0);
            }
        }
    }
}
