using Microsoft.Extensions.DependencyInjection;
using MOnGoL.Common;

namespace MOnGoL.Backend
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPlayersService, PlayersService>();
            services.AddSingleton<IBoardService, GameOfLifeBoardService>();
            services.AddScoped<IPlayerService, PlayerService>();
        }
    }
}