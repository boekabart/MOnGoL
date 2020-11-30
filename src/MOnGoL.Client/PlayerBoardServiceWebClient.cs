using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{
    public class PlayerBoardServiceWebClient : IPlayerBoardService
    {
        public PlayerBoardServiceWebClient(NavigationManager client)
        {
            var uri = client.ToAbsoluteUri("/api/hubs/playerBoard");
            hubConnection = new HubConnectionBuilder()
            .WithUrl(uri)
            .Build();

            hubConnection.On<ChangeSet>("BoardChanged", changes =>
            {
                ApplyChanges(changes);
                OnBoardChanged?.Invoke(this, changes);
            });
        }

        private void ApplyChanges(ChangeSet changes)
        {
            _theBoard = _theBoard?.WithChanges(changes);
        }

        private EventHandler<ChangeSet> onBoardChanged;
        private HubConnection hubConnection;
        private Board _theBoard;

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
            return await hubConnection.InvokeAsync<bool>("TryPlaceToken", where);
        }

        public async Task<Board> GetBoard()
        {
            if (_theBoard is not null)
                return _theBoard;
            await Connect();
            _theBoard = await hubConnection.InvokeAsync<Board>("GetBoard");
            //OnBoardChanged?.Invoke(this, new ChangeSet(ImmutableList<Change>.Empty));
            return _theBoard;
        }

        private async Task Connect()
        {
            if (hubConnection.State != HubConnectionState.Connected)
            {
                await hubConnection.StartAsync();
                await GetBoard();
            }
        }
    }
}
