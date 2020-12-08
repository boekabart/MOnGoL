﻿using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class PlayerService : IPlayerService, IAsyncDisposable
    {
        private ILogger<PlayerService> Logger { get; set; }
        public PlayerService(IPlayersService playersService, ILogger<PlayerService> logger)
        {
            PlayersService = playersService;
            Logger = logger;
            PlayersService.OnPlayerlistChanged += OnGlobalPlayerlistChanged;
            Logger.LogInformation("Created");
        }

        private void OnGlobalPlayerlistChanged(object sender, IImmutableList<PlayerState> newPlayerlist)
        {
            OnPlayerlistChanged?.Invoke(this, newPlayerlist);
        }

        public EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged { get; set; }
        public EventHandler<PlayerInfo?> OnMyInfoChanged { get; set;  }
        private IPlayersService PlayersService { get; }
        private PlayerInfo? _myInfo;

        public Task<PlayerInfo> GetMyInfo()
        {
            return Task.FromResult(_myInfo);
        }

        public Task<IImmutableList<PlayerState>> GetPlayerlist()
        {
            return PlayersService.GetPlayerlist();
        }

        public async Task Leave()
        {
            Logger.LogInformation("Leave()");
            if (_myInfo is not null)
            {
                await PlayersService.Leave(_myInfo);
                _myInfo = null;
                OnMyInfoChanged?.Invoke(this, _myInfo);
            }
        }

        public async Task<PlayerInfo> Register(PlayerInfo myInfo)
        {
            Logger.LogInformation("Register");
            if (_myInfo is not null)
                return _myInfo;
            Logger.LogInformation("Registering with PS");
            _myInfo = await PlayersService.Register(myInfo);
            if (_myInfo is not null)
                OnMyInfoChanged?.Invoke(this, _myInfo);
            return _myInfo;
        }

        public async ValueTask DisposeAsync()
        {
            Logger.LogInformation("Dispose()");
            await Leave();
            PlayersService.OnPlayerlistChanged -= OnGlobalPlayerlistChanged;
        }
    }
}
