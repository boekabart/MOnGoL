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
        private ImmutableList<PlayerState> _playerlist = ImmutableList<PlayerState>.Empty;

        public EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged { get; set; }

        public async Task<IImmutableList<PlayerState>> GetPlayerlist()
        {
            using var _ = await Lock();
            return _playerlist;
        }

        public async Task Leave(PlayerInfo myInfo)
        {
            using var _ = await Lock();
            var index = _playerlist.FindIndex(ps => ps.PlayerInfo.Equals(myInfo));
            if (index < 0)
                return;

            _playerlist = _playerlist.RemoveAt(index);
            OnPlayerlistChanged?.Invoke(this, _playerlist);
            Logger.LogInformation($"Player {myInfo.Name} left; {_playerlist.Count} in list");
        }

        public async Task<PlayerInfo?> Register(PlayerInfo myInfo)
        {
            using var _ = await Lock();
            if (_playerlist.Any(ps => ps.PlayerInfo.Name.Equals(myInfo.Name) || ps.PlayerInfo.Token.Equals(myInfo.Token)))
                return null;
            _playerlist = _playerlist.Add(new PlayerState(myInfo,0));
            OnPlayerlistChanged?.Invoke(this, _playerlist);
            Logger.LogInformation($"Player {myInfo.Name} joined; {_playerlist.Count} in list");
            return myInfo;
        }

        public async Task<bool> Score(Token playerToken, int delta)
        {
            if (delta == 0)
                return true;

            using var _ = await Lock();
            var index = _playerlist.FindIndex(ps => ps.PlayerInfo.Token.Emoji.Equals(playerToken.Emoji));
            if (index < 0)
                return false;

            var newPlayerState = _playerlist[index] with { Score = _playerlist[index].Score + delta };
            _playerlist = _playerlist.SetItem(index, newPlayerState);
            OnPlayerlistChanged?.Invoke(this, _playerlist);
            return true;
        }

        private Task<IDisposable> Lock()
        {
            return _lock.DisposableEnter();
        }
    }
}
