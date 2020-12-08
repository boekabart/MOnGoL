using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Controller.Hubs
{

    public class PlayerHub : Hub
    {
        public Service HubService { get; }

        public class Service : IDisposable
        {
            private readonly IHubContext<PlayerHub> hubContext;
            private readonly IPlayersService playersService;
            private readonly IBoardService boardService;

            public Service(IHubContext<PlayerHub> hubContext, IPlayersService playersService, IBoardService boardService)
            {
                this.hubContext = hubContext;
                this.playersService = playersService;
                this.boardService = boardService;
                playersService.OnPlayerlistChanged += OnPlayerlistChanged;
                boardService.OnBoardChanged += OnBoardChanged;
            }

            private async void OnBoardChanged(object sender, ChangeSet changeSet)
            {
                await hubContext.Clients.All.SendAsync("BoardChanged", changeSet);
            }

            public void Dispose()
            {
                boardService.OnBoardChanged -= OnBoardChanged;
                playersService.OnPlayerlistChanged -= OnPlayerlistChanged;
            }

            private async void OnPlayerlistChanged(object sender, IImmutableList<PlayerState> e)
            {
                await hubContext.Clients.All.SendAsync("PlayerlistChanged", e);
            }
        }

        public SignalRScopeService ScopeService { get; }

        public PlayerHub(SignalRScopeService scopeService, Service _)
        {
            ScopeService = scopeService;
        }

        public async Task<PlayerInfo> GetMyInfo()
        {
            var playerService = await GetPlayerService();
            return await playerService.GetMyInfo();
        }

        public async Task<IImmutableList<PlayerState>> GetPlayerlist()
        {
            var playerService = await GetPlayerService();
            return await playerService.GetPlayerlist();
        }

        public async Task Leave()
        {
            var playerService = await GetPlayerService();
            await playerService.Leave();
        }

        public async Task<PlayerInfo> Register(PlayerInfo myInfo)
        {
            var playerService = await GetPlayerService();
            return await playerService.Register(myInfo);
        }

        public async Task<bool> TryPlaceToken(Coordinate where)
        {
            var playerBoardService = await GetPlayerBoardService();
            return await playerBoardService.TryPlaceToken(where);
        }

        public async Task<Board> GetBoard()
        {
            var playerBoardService = await GetPlayerBoardService();
            return await playerBoardService.GetBoard();
        }

        private async Task<IPlayerBoardService> GetPlayerBoardService()
        {
            return (await ScopeService.GetScope(Context)).GetRequiredService<IPlayerBoardService>();
        }

        private async Task<IPlayerService> GetPlayerService()
        {
            return (await ScopeService.GetScope(Context)).GetRequiredService<IPlayerService>();
        }
    }
}
