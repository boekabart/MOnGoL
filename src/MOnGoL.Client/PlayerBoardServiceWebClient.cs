using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{
    public class PlayerBoardServiceWebClient : IPlayerBoardService
    {
        public PlayerBoardServiceWebClient(SignalRConnection signalR)
        {
            SignalR = signalR;
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

        private EventHandler<ChangeSet> onBoardChanged;
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
            return await HubConnection.InvokeAsync<bool>("TryPlaceToken", where);
        }

        public async Task<Board> GetBoard()
        {
            if (_theBoard is not null)
                return _theBoard;
            await Connect();
            _theBoard = await HubConnection.InvokeAsync<Board>("GetBoard");
            //OnBoardChanged?.Invoke(this, new ChangeSet(ImmutableList<Change>.Empty));
            return _theBoard;
        }

        public SignalRConnection SignalR { get; }
        private HubConnection HubConnection => SignalR.HubConnection;
        private Task Connect()
        {
            return SignalR.Connect();
        }
    }
}
