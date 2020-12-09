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
            SignalR.HubConnection.On<int>("OnTokenStockChanged", newValue =>
            {
                OnTokenStockChanged?.Invoke(this, newValue);
            });
            HubConnection.On<ChangeSet>("BoardChanged", changes =>
            {
                ApplyChanges(changes);
                OnBoardChanged?.Invoke(this, changes);
            });
        }

        private void ApplyChanges(ChangeSet changes)
        {
            _theBoard = _theBoard?.WithChanges(changes);
        }

        private Board _theBoard;

        private EventHandler<int> onTokenStockChanged;
        public EventHandler<int> OnTokenStockChanged
        {
            get => onTokenStockChanged;
            set
            {
                onTokenStockChanged = value;
                _ = Connect();
            }
        }

        private EventHandler<ChangeSet> onBoardChanged;
        public EventHandler<ChangeSet> OnBoardChanged
        {
            get => onBoardChanged;
            set
            {
                onBoardChanged = value;
                _ = Connect();
            }
        }

        public async Task<bool> TryPlaceToken(Coordinate where)
        {
            await Connect();
            return await HubConnection.InvokeAsync<bool>("TryPlaceToken", where);
        }

        public async Task<Board> GetBoard()
        {
            if (_theBoard is not null)
                return _theBoard;
            await Connect();
            _theBoard = await HubConnection.InvokeAsync<Board>("GetBoard");
            return _theBoard;
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
            await HubConnection.InvokeAsync("Leave");
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

        public async Task<int> GetTokenStock()
        {
            await Connect();
            return await HubConnection.InvokeAsync<int>("GetTokenStock");
        }
    }
}
