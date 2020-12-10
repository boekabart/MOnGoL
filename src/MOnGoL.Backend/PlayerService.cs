using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class PlayerService : IPlayerService, IAsyncDisposable
    {
        private ILogger<PlayerService> Logger { get; set; }
        public PlayerService(IPlayersService playersService, ILogger<PlayerService> logger, IBoardService boardService)
        {
            PlayersService = playersService;
            Logger = logger;
            PlayersService.OnPlayerlistChanged += OnGlobalPlayerlistChanged;
            Logger.LogInformation("Created");
            BoardService = boardService;
            BoardService.OnBoardChanged += OnGlobalBoardChanged;
            BoardService.OnCountdownChanged += OnGlobalCountdownChanged;
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
            using var _ = await Lock();
            if (_myInfo is not null)
            {
                await PlayersService.Leave(_myInfo);
                _myInfo = null;
                OnMyInfoChanged?.Invoke(this, _myInfo);
                TokenStock = 0;
            }
        }

        public async Task<PlayerInfo> Register(PlayerInfo myInfo)
        {
            using var _ = await Lock();
            Logger.LogInformation("Register");
            if (_myInfo is not null)
                return _myInfo;
            Logger.LogInformation("Registering with PS");
            _myInfo = await PlayersService.Register(myInfo);
            if (_myInfo is not null)
            {
                OnMyInfoChanged?.Invoke(this, _myInfo);
                TokenStock = MaxTokenStock;
            }
            return _myInfo;
        }

        public async ValueTask DisposeAsync()
        {
            Logger.LogInformation("Dispose()");
            await Leave();
            PlayersService.OnPlayerlistChanged -= OnGlobalPlayerlistChanged;
            BoardService.OnBoardChanged -= OnGlobalBoardChanged;
            BoardService.OnCountdownChanged -= OnGlobalCountdownChanged;
        }

        #region TokenStock
        public EventHandler<int> OnTokenStockChanged { get; set; }

        public async Task<int> GetTokenStock()
        {
            using var _ = await Lock();
            return TokenStock;
        }

        public async Task AddTokenToStock()
        {
            using var _ = await Lock();
            if (_myInfo is null)
                return;

            if (TokenStock >= MaxTokenStock)
                return;

            TokenStock++;
        }

        private int tokenStock;
        private int TokenStock
        {
            get => tokenStock;
            set
            {
                tokenStock = value;
                OnTokenStockChanged?.Invoke(this, tokenStock);
            }
        }

        public async Task<bool> TryPopTokenFromStock()
        {
            using var _ = await Lock();

            if (TokenStock <= 0)
                return false;

            TokenStock--;
            return true;
        }

        private async void ScheduleAddingTokenToStock()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            await AddTokenToStock();
        }

        private readonly static int MaxTokenStock = 3;
        private SemaphoreSlim _lock = new SemaphoreSlim(1);
        private Task<IDisposable> Lock() => _lock.DisposableEnter();
        #endregion

        #region Board Stuff
        private IBoardService BoardService { get; init; }

        private void OnGlobalBoardChanged(object sender, ChangeSet changeSet)
        {
            OnBoardChanged?.Invoke(this, changeSet);
        }

        public EventHandler<ChangeSet> OnBoardChanged { get; set; }

        private void OnGlobalCountdownChanged(object sender, int countDown)
        {
            OnCountdownChanged?.Invoke(this, countDown);
        }

        public EventHandler<int> OnCountdownChanged { get; set; }

        public Task<Board> GetBoard()
        {
            return BoardService.GetBoard();
        }

        public async Task<bool> TryPlaceToken(Coordinate where)
        {
            if (!await TryPopTokenFromStock())
                return false;

            var success = await BoardService.TryPlaceToken(where, _myInfo.Token);
            if (!success)
            {
                await AddTokenToStock();
            }
            else
            {
                ScheduleAddingTokenToStock();
                await PlayersService.Score(_myInfo.Token, -1);
            }
            return success;
        }
        #endregion
    }
}
