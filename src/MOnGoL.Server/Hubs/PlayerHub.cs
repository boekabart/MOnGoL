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
        public class BroadcastService : IDisposable
        {
            private readonly IHubContext<PlayerHub> hubContext;
            private readonly IPlayerService playerService;

            public BroadcastService(IHubContext<PlayerHub> hubContext, IPlayerService playerService)
            {
                this.hubContext = hubContext;
                this.playerService = playerService;
                playerService.OnPlayerlistChanged += OnPlayerlistChanged;
                playerService.OnMyInfoChanged += OnMyInfoChanged;
                playerService.OnBoardChanged += OnBoardChanged;
                playerService.OnTokenStockChanged += OnTokenStockChanged;
            }

            public void Dispose()
            {
                playerService.OnTokenStockChanged -= OnTokenStockChanged;
                playerService.OnBoardChanged -= OnBoardChanged;
                playerService.OnMyInfoChanged -= OnMyInfoChanged;
                playerService.OnPlayerlistChanged -= OnPlayerlistChanged;
            }

            private async void OnBoardChanged(object sender, ChangeSet changeSet)
            {
                await hubContext.Clients.All.SendAsync("BoardChanged", changeSet);
            }

            private async void OnTokenStockChanged(object sender, int newStockValue)
            {
                await hubContext.Clients.All.SendAsync("OnTokenStockChanged", newStockValue);
            }

            private async void OnMyInfoChanged(object sender, PlayerInfo? e)
            {
                await hubContext.Clients.All.SendAsync("OnMyInfoChanged", e);
            }

            private async void OnPlayerlistChanged(object sender, IImmutableList<PlayerState> e)
            {
                await hubContext.Clients.All.SendAsync("PlayerlistChanged", e);
            }
        }

        public SignalRScopeService ScopeService { get; }

        public PlayerHub(SignalRScopeService scopeService)
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
            var playerService = await GetPlayerService();
            return await playerService.TryPlaceToken(where);
        }

        public async Task<Board> GetBoard()
        {
            var playerService = await GetPlayerService();
            return await playerService.GetBoard();
        }

        public async Task<int> GetTokenStock()
        {
            var playerService = await GetPlayerService();
            return await playerService.GetTokenStock();
        }

        private async Task<IPlayerService> GetPlayerService()
        {
            return (await GetScope()).GetRequiredService<IPlayerService>();
        }

        private async Task<IServiceProvider> GetScope()
        {
            var serviceProvider = await ScopeService.GetScope(Context);
            var _ = serviceProvider.GetRequiredService<BroadcastService>();
            return serviceProvider;
        }
    }
}
