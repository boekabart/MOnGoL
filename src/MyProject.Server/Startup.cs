using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Backend.Controller.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Backend.Controller
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services
                .AddControllers()
                .AddApplicationPart(typeof(Startup).Assembly);

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddSingleton<CounterHubService>();
            return services;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CounterHub>("/hubs/counter");
            });
        }
    }
}
