using Microsoft.AspNetCore.SignalR;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Controller.Hubs
{
    public class PlayerHub : Hub
    {
        public class Service : IDisposable
        {
            private readonly IHubContext<PlayerHub> hubContext;
            private readonly IPlayerService playerService;

            public Service(IHubContext<PlayerHub> hubContext, IPlayerService playerService)
            {
                this.hubContext = hubContext;
                this.playerService = playerService;
                playerService.OnPlayerlistChanged += OnPlayerlistChanged;
            }

            public void Dispose()
            {
                playerService.OnPlayerlistChanged -= OnPlayerlistChanged;
            }

            private async void OnPlayerlistChanged(object sender, IImmutableList<PlayerInfo> e)
            {
                await hubContext.Clients.All.SendAsync("PlayerlistChanged", e);
            }
        }

        private readonly IPlayerService playerService;

        public PlayerHub( IPlayerService playerService, Service _)
        {
            this.playerService = playerService;
        }

        public Task<PlayerInfo> GetMyInfo()
        {
            return playerService.GetMyInfo();
        }

        public Task<IImmutableList<PlayerInfo>> GetPlayerlist()
        {
            return playerService.GetPlayerlist();
        }

        public Task Leave()
        {
            return playerService.Leave();
        }

        public Task<PlayerInfo> Register(PlayerInfo myInfo)
        {
            return playerService.Register(myInfo);
        }
    }
}
