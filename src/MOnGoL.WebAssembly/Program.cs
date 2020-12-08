using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MOnGoL.WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Frontend.App>("#app");

            Frontend.Startup.ConfigureServices(builder.Services);
            var uri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiUri = uri;// new Uri($"http://{uri.Host}:5000/");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUri });
            builder.Services.AddScoped<Backend.Client.SignalRConnection>();
            builder.Services.AddScoped<Common.IPlayerService, Backend.Client.PlayerServiceWebClient>();
            builder.Services.AddScoped<Common.IPlayerBoardService, Backend.Client.PlayerBoardServiceWebClient>();

            await builder.Build().RunAsync();
        }
    }
}
