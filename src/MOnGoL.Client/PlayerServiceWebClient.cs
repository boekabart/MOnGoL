using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{
    public class PlayerServiceWebClient : IPlayerService
    {
        public PlayerServiceWebClient(NavigationManager client)
        {
            var uri = client.ToAbsoluteUri("/api/hubs/player");
            hubConnection = new HubConnectionBuilder()
            .WithUrl(uri)
            .Build();

            hubConnection.On<IImmutableList<PlayerInfo>>("PlayerlistChanged", newValue =>
            {
                lastPlayerlist = newValue;
                OnPlayerlistChanged?.Invoke(this, newValue);
            });
        }

        private EventHandler<IImmutableList<PlayerInfo>> onPlayerlistChanged;
        private HubConnection hubConnection;
        private IImmutableList<PlayerInfo> lastPlayerlist;
        private Task connectTask;

        public EventHandler<IImmutableList<PlayerInfo>> OnPlayerlistChanged
        {
            get => onPlayerlistChanged;
            set
            {
                onPlayerlistChanged = value;
                _ = Connect();
                if (lastPlayerlist is not null)
                    onPlayerlistChanged?.Invoke(this, lastPlayerlist);
            }
        }

        public async Task<PlayerInfo?> GetMyInfo()
        {
            await Connect();
            return await hubConnection.InvokeAsync<PlayerInfo?>("GetMyInfo");
        }

        public async Task<IImmutableList<PlayerInfo>> GetPlayerlist()
        {
            await Connect();
            return await hubConnection.InvokeAsync<IImmutableList<PlayerInfo>>("GetPlayerlist");
        }

        public async Task Leave()
        {
            await Connect();
            await hubConnection.SendAsync("Leave");
            await hubConnection.StopAsync();
        }

        public async Task<PlayerInfo?> Register(PlayerInfo myInfo)
        {
            await Connect();
            return await hubConnection.InvokeAsync<PlayerInfo?>("Register", myInfo);
        }

        private async Task Connect()
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                return;
            }
            else if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await (connectTask = hubConnection.StartAsync());
            }
            else if (connectTask is not null)
            {
                await connectTask;
            }
        }
    }
}
