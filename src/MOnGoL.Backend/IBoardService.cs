using MOnGoL.Common;
using System;
using System.Threading.Tasks;

namespace MOnGoL.Backend
{
    public interface IBoardService
    {
        Task<bool> TryPlaceToken(Coordinate where, Token token);
        EventHandler<ChangeSet> OnBoardChanged { get; set; }
        EventHandler<int> OnCountdownChanged { get; set; }

        Task<Board> GetBoard();
    }
}

