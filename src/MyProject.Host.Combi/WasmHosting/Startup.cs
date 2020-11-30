using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;

namespace MyProject.Host.Combi.WasmHosting
{
    public static class Startup
    {
        internal static void ConfigureServices(IServiceCollection _)
        {
        }

        public static void Configure(IApplicationBuilder wasm, IWebHostEnvironment _)
        {
            wasm.UseWebAssemblyDebugging();
            wasm.UseRewriter(new RewriteOptions().AddRedirect(@"^$", "/home", 301));

            wasm.UseBlazorFrameworkFiles();
            wasm.UseStaticFiles();

            wasm.UseRouting();

            wasm.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
