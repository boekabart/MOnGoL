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

            public Service(IHubContext<PlayerHub> hubContext, IPlayersService playersService)
            {
                this.hubContext = hubContext;
                this.playersService = playersService;
                playersService.OnPlayerlistChanged += OnPlayerlistChanged;
            }

            public void Dispose()
            {
                playersService.OnPlayerlistChanged -= OnPlayerlistChanged;
            }

            private async void OnPlayerlistChanged(object sender, IImmutableList<PlayerInfo> e)
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

        public async Task<IImmutableList<PlayerInfo>> GetPlayerlist()
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

        private async Task<IPlayerService> GetPlayerService()
        {
            return (await ScopeService.GetScope(Context)).GetRequiredService<IPlayerService>();
        }
    }
}
