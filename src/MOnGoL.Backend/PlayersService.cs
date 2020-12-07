using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class PlayersService : IPlayersService
    {
        private ILogger<PlayerService> Logger { get; set; }

        public PlayersService(ILogger<PlayerService> logger)
        {
            Logger = logger;
        }

        private SemaphoreSlim _lock = new SemaphoreSlim(1);
        private ImmutableList<PlayerInfo> _playerlist = ImmutableList<PlayerInfo>.Empty;

        public EventHandler<IImmutableList<PlayerInfo>> OnPlayerlistChanged { get; set; }

        public async Task<IImmutableList<PlayerInfo>> GetPlayerlist()
        {
            using var _ = await Lock();
            return _playerlist;
        }

        public async Task Leave(PlayerInfo myInfo)
        {
            using var _ = await Lock();
            _playerlist = _playerlist.Remove(myInfo);
            OnPlayerlistChanged?.Invoke(this, _playerlist);
            Logger.LogInformation($"Player {myInfo.Name} left; {_playerlist.Count} in list");
        }

        public async Task<PlayerInfo?> Register(PlayerInfo myInfo)
        {
            using var _ = await Lock();
            if (_playerlist.Any(pi => pi.Name.Equals(myInfo.Name) || pi.Token.Equals(myInfo.Token)))
                return null;
            _playerlist = _playerlist.Add(myInfo);
            OnPlayerlistChanged?.Invoke(this, _playerlist);
            Logger.LogInformation($"Player {myInfo.Name} joined; {_playerlist.Count} in list");
            return myInfo;
        }
        private Task<IDisposable> Lock()
        {
            return _lock.DisposableEnter();
        }
    }
}
