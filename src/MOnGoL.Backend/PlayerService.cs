using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class PlayerService : IPlayerService, IDisposable
    {
        public PlayerService(IPlayersService playersService)
        {
            PlayersService = playersService;
            PlayersService.OnPlayerlistChanged += OnGlobalPlayerlistChanged;
        }

        private void OnGlobalPlayerlistChanged(object sender, IImmutableList<PlayerInfo> newPlayerlist)
        {
            OnPlayerlistChanged?.Invoke(this, newPlayerlist);
        }

        public EventHandler<IImmutableList<PlayerInfo>> OnPlayerlistChanged { get; set;  }
        private IPlayersService PlayersService { get; }
        private PlayerInfo? _myInfo;

        public void Dispose()
        {
            PlayersService.OnPlayerlistChanged -= OnGlobalPlayerlistChanged;
        }

        public Task<PlayerInfo> GetMyInfo()
        {
            return Task.FromResult(_myInfo);
        }

        public Task<IImmutableList<PlayerInfo>> GetPlayerlist()
        {
            return PlayersService.GetPlayerlist();
        }

        public async Task Leave()
        {
            if (_myInfo is not null)
                await PlayersService.Leave(_myInfo);
        }

        public async Task<PlayerInfo> Register(PlayerInfo myInfo)
        {
            if (_myInfo is not null)
                return _myInfo;
            return _myInfo = await PlayersService.Register(myInfo);
        }
    }
}
