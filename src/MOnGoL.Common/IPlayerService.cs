using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IPlayerService
    {
        #region Actions

        Task<PlayerInfo> Register(PlayerInfo myInfo);
        Task Leave();

        Task<bool> TryPlaceToken(Coordinate where);

        #endregion

        Task<IImmutableList<PlayerState>> GetPlayerlist();
        EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged { get; set; }

        Task<PlayerInfo> GetMyInfo();
        EventHandler<PlayerInfo> OnMyInfoChanged { get; set; }

        Task<Board> GetBoard();
        EventHandler<ChangeSet> OnBoardChanged { get; set; }

        Task<int> GetTokenStock();
        EventHandler<int> OnTokenStockChanged { get; set; }

        EventHandler<int> OnCountdownChanged { get; set; }
    }
}
