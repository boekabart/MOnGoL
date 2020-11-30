using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyProject.WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Frontend.App>("#app");

            var uri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiUri = uri;// new Uri($"http://{uri.Host}:5000/");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUri });
            builder.Services.AddScoped<Common.IWeatherForecastService, Backend.Client.WeatherForecastServiceWebClient>();
            builder.Services.AddScoped<Common.ICounterService, Backend.Client.CounterServiceWebClient>();

            await builder.Build().RunAsync();
        }
    }
}
