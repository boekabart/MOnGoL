using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MyProject.Common;

namespace MyProject.Backend.Controller.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ILogger<WeatherForecastController> Logger { get; set; }
        private IWeatherForecastService WeatherForecastService { get; set; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
        {
            Logger = logger;
            WeatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromQuery][BindRequired] DateTime startDate)
        {
            Logger.LogInformation($"Got a request for forecasts from {startDate}");
            return await WeatherForecastService.GetForecastAsync(startDate);
        }
    }
}
