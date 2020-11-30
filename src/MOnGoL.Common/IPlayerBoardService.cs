using System;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IPlayerBoardService
    {
        Task<bool> TryPlaceToken(Coordinate where);
        EventHandler<ChangeSet> OnBoardChanged { get; set; }
        Task<Board> GetBoard();
    }
}

