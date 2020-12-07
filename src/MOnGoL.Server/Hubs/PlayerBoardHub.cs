using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MOnGoL.Common;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Controller.Hubs
{
    public class PlayerBoardHub : Hub
    {
        public class Service
        {
            private readonly IHubContext<PlayerBoardHub> hubContext;
            private readonly IBoardService boardService;

            public Service(IHubContext<PlayerBoardHub> hubContext, IBoardService boardService)
            {
                this.hubContext = hubContext;
                this.boardService = boardService;
                boardService.OnBoardChanged += OnBoardChanged;
            }

            public void Dispose()
            {
                boardService.OnBoardChanged -= OnBoardChanged;
            }

            private async void OnBoardChanged(object sender, ChangeSet changeSet)
            {
                await hubContext.Clients.All.SendAsync("BoardChanged", changeSet);
            }
        }

        public SignalRScopeService ScopeService { get; }

        public PlayerBoardHub(SignalRScopeService scopeService, Service _)
        {
            ScopeService = scopeService;
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
    }
}
