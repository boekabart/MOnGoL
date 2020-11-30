using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IPlayerService
    {
        EventHandler<IImmutableList<PlayerInfo>> OnPlayerlistChanged { get; set; }
        Task<IImmutableList<PlayerInfo>> GetPlayerlist();
        Task<PlayerInfo?> GetMyInfo();
        Task<PlayerInfo?> Register(PlayerInfo myInfo);
        Task Leave();
    }
}

