using System;

namespace MyProject.Common
{
    public interface IWeatherForecastService
    {
        System.Threading.Tasks.Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}

