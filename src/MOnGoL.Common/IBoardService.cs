using System;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IBoardService
    {
        Task<bool> TryPlaceToken(Coordinate where, Token token);
        EventHandler<ChangeSet> OnBoardChanged { get; set; }

        Task<Board> GetBoard();
    }
}

