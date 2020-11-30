using MyProject.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Backend.Client
{

    public class WeatherForecastServiceWebClient : IWeatherForecastService
    {
        public WeatherForecastServiceWebClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var response = HttpClient.GetFromJsonAsync<WeatherForecast[]>($"/api/WeatherForecast?startDate={startDate.ToUniversalTime():o}");
            return response;
        }
    }
}
