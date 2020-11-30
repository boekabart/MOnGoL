using Microsoft.Extensions.DependencyInjection;
using MOnGoL.Common;

namespace MOnGoL.Backend
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
            services.AddSingleton<ICounterService, CounterService>();
        }
    }
}