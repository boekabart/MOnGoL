using Microsoft.Extensions.DependencyInjection;
using MyProject.Common;

namespace MyProject.Backend
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