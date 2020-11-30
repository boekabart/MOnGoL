using System;

namespace MOnGoL.Common
{
    public interface IWeatherForecastService
    {
        System.Threading.Tasks.Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}

