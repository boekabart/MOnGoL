using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface IPlayersService
    {
        EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged { get; set; }
        Task<IImmutableList<PlayerState>> GetPlayerlist();
        Task<bool> Score(Token playerToken);
        Task<PlayerInfo?> Register(PlayerInfo myInfo);
        Task Leave(PlayerInfo myInfo);
    }
}

