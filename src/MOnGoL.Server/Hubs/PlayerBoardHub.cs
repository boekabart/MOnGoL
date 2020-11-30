using Microsoft.AspNetCore.SignalR;
using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Controller.Hubs
{
    public class PlayerBoardHub : Hub
    {
        public class Service : IDisposable
        {
            private readonly IHubContext<PlayerBoardHub> hubContext;
            private readonly IPlayerBoardService playerBoardService;

            public Service(IHubContext<PlayerBoardHub> hubContext, IPlayerBoardService playerBoardService)
            {
                this.hubContext = hubContext;
                this.playerBoardService = playerBoardService;
                playerBoardService.OnBoardChanged += OnBoardChanged;
            }

            public void Dispose()
            {
                playerBoardService.OnBoardChanged -= OnBoardChanged;
            }

            private async void OnBoardChanged(object sender, ChangeSet changeSet)
            {
                await hubContext.Clients.All.SendAsync("BoardChanged", changeSet);
            }
        }

        private readonly IPlayerBoardService playerBoardService;

        public PlayerBoardHub(IPlayerBoardService playerBoardService, Service _)
        {
            this.playerBoardService = playerBoardService;
        }

        public Task<bool> TryPlaceToken(Coordinate where)
        {
            return playerBoardService.TryPlaceToken(where);
        }

        public Task<Board> GetBoard()
        {
            return playerBoardService.GetBoard();
        }
    }
}
