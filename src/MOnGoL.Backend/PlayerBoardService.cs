using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public class PlayerBoardService : IPlayerBoardService, IDisposable
    {
        private readonly IPlayerService playerService;
        private readonly IBoardService boardService;

        public PlayerBoardService(IPlayerService playerService, IBoardService boardService)
        {
            this.playerService = playerService;
            this.boardService = boardService;
            this.boardService.OnBoardChanged += OnGlobalBoardChanged;
        }

        private void OnGlobalBoardChanged(object sender, ChangeSet changeSet)
        {
            OnBoardChanged?.Invoke(this, changeSet);
        }

        public EventHandler<ChangeSet> OnBoardChanged { get; set; }

        public void Dispose()
        {
            this.boardService.OnBoardChanged -= OnGlobalBoardChanged;
        }

        public Task<Board> GetBoard()
        {
            return boardService.GetBoard();
        }

        public async Task<bool> TryPlaceToken(Coordinate where)
        {
            var myInfo = await playerService.GetMyInfo();
            if (myInfo is null)
                return false;

            return await boardService.TryPlaceToken(where, myInfo.Token);
        }
    }
}
