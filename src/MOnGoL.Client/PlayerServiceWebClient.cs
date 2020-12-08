using Microsoft.AspNetCore.SignalR.Client;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{
    public class PlayerServiceWebClient : IPlayerService
    {
        public PlayerServiceWebClient(SignalRConnection signalR)
        {
            SignalR = signalR;
            SignalR.HubConnection.On<IImmutableList<PlayerState>>("PlayerlistChanged", newValue =>
            {
                OnPlayerlistChanged?.Invoke(this, newValue);
            });
            SignalR.HubConnection.On<PlayerInfo?>("OnMyInfoChanged", newValue =>
            {
                OnMyInfoChanged?.Invoke(this, newValue);
            });
        }

        private EventHandler<IImmutableList<PlayerState>> onPlayerlistChanged;

        public EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged
        {
            get => onPlayerlistChanged;
            set
            {
                onPlayerlistChanged = value;
                _ = SignalR.Connect();
            }
        }

        private EventHandler<PlayerInfo?> onMyInfoChanged;

        public EventHandler<PlayerInfo?> OnMyInfoChanged
        {
            get => onMyInfoChanged;
            set
            {
                onMyInfoChanged = value;
                _ = SignalR.Connect();
            }
        }

        public async Task<PlayerInfo?> GetMyInfo()
        {
            await SignalR.Connect();
            return await HubConnection.InvokeAsync<PlayerInfo?>("GetMyInfo");
        }

        public async Task<IImmutableList<PlayerState>> GetPlayerlist()
        {
            await SignalR.Connect();
            return await HubConnection.InvokeAsync<IImmutableList<PlayerState>>("GetPlayerlist");
        }

        public async Task Leave()
        {
            await SignalR.Connect();
            await HubConnection.SendAsync("Leave");
            await HubConnection.StopAsync();
        }

        public async Task<PlayerInfo?> Register(PlayerInfo myInfo)
        {
            await Connect();
            return await HubConnection.InvokeAsync<PlayerInfo?>("Register", myInfo);
        }

        public SignalRConnection SignalR { get; }
        private HubConnection HubConnection => SignalR.HubConnection;
        private Task Connect()
        {
            return SignalR.Connect();
        }
    }
}
