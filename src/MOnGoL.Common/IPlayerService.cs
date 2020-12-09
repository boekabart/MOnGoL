using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IPlayerService
    {
        EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged { get; set; }
        EventHandler<PlayerInfo> OnMyInfoChanged { get; set; }
        Task<IImmutableList<PlayerState>> GetPlayerlist();
        Task<PlayerInfo> GetMyInfo();
        Task<PlayerInfo> Register(PlayerInfo myInfo);
        Task Leave();

        #region Board
        Task<bool> TryPlaceToken(Coordinate where);
        EventHandler<ChangeSet> OnBoardChanged { get; set; }
        EventHandler<int> OnTokenStockChanged { get; set; }
        Task<Board> GetBoard();
        Task<int> GetTokenStock();
        #endregion
    }
}

